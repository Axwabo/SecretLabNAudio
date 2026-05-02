#if DEBUG
using Random = System.Random;
#else
using Random = UnityEngine.Random;
#endif
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Extensions.Processors;

namespace SecretLabNAudio.Core.Processors.Playlists;

/// <summary>
/// A lazily-evaluated playlist that plays items one after another.
/// Sample providers are only created when each item starts, which saves memory, and reduces open file handles.
/// </summary>
public sealed partial class LazyPlaylist : IAudioProcessor
{

    private readonly List<PlaylistItem> _items = [];

    private (PlaylistItem Item, ISampleProvider Provider)? _current;

    public int Index { get; private set; }

    public PlaylistState State { get; private set; }

    /// <summary>
    /// A read-only view of the items in the playlist.
    /// </summary>
    public IReadOnlyList<PlaylistItem> Items { get; }

    /// <summary>
    /// Whether to shuffle items when the playlist (re)starts.
    /// </summary>
    public bool ShuffleOnStart { get; set; }

    public Repeat RepeatMode
    {
        get;
        set
        {
            field = value;
            GetSource<ILoopable>()?.Loop = value == Repeat.One;
        }
    }

    /// <summary>
    /// The item currently being played, if any.
    /// </summary>
    public PlaylistItem? CurrentItem => _current?.Item;

    private bool IsPlaying => State is PlaylistState.PlayingIndex or PlaylistState.MovingToNextItem;

    private bool NextAvailable => Index < _items.Count - 1;

    public event Action? BeforeStarted;

    public event Action? CurrentItemChanged;

    public event Action? LastItemEnded;

    public LazyPlaylist(WaveFormat waveFormat)
    {
        WaveFormat = waveFormat;
        Items = _items.AsReadOnly();
    }

    public LazyPlaylist(WaveFormat waveFormat, params IEnumerable<PlaylistItem> items) : this(waveFormat) => _items.AddRange(items);

    /// <inheritdoc/>
    public int Read(float[] buffer, int offset, int count)
    {
        if (State == PlaylistState.Ended)
            return 0;
        var total = 0;
        while (total < count && TryGetCurrent(out var provider))
        {
            var target = Math.Max(0, count - total);
            var read = provider.Read(buffer, total + offset, target);
            total += read;
            if (read < target)
                State = PlaylistState.MovingToNextItem;
        }

        return total;
    }

    private bool TryGetCurrent([NotNullWhen(true)] out ISampleProvider? provider)
    {
        switch (State, RepeatMode)
        {
            case (PlaylistState.NotStarted, _):
            case (PlaylistState.MovingToNextItem, Repeat.All) when !NextAvailable:
                return Restart(out provider);
            case (PlaylistState.MovingToNextItem, Repeat.One):
                return Next(false, out provider);
            case (PlaylistState.MovingToNextItem, _):
                if (NextAvailable)
                    return Next(true, out provider);
                End(true);
                provider = null;
                return false;
            default:
                provider = _current?.Provider;
                return provider != null;
        }
    }

    private bool Restart([NotNullWhen(true)] out ISampleProvider? provider, bool? shuffle = null)
    {
        EndCurrent();
        _current = null;
        if (_items.Count == 0)
        {
            State = PlaylistState.Ended;
            provider = null;
            return false;
        }

        State = PlaylistState.NotStarted;
        if (shuffle ?? ShuffleOnStart)
            Shuffle();
        BeforeStarted.InvokeSafely();
        Index = 0;
        return Next(false, out provider);
    }

    private bool Next(bool advance, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        var wasPlaying = IsPlaying;
        if (advance && NextAvailable)
            Index++;
        while (Index < _items.Count)
        {
            if (!Begin(_items[Index], out provider))
            {
                Index++;
                continue;
            }

            State = PlaylistState.PlayingIndex;
            CurrentItemChanged.InvokeSafely();
            return true;
        }

        End(wasPlaying);
        provider = null;
        return false;
    }

    /// <summary>
    /// Randomizes the order of items in-place.
    /// </summary>
    /// <returns>The playlist itself.</returns>
    private void Shuffle()
    {
#if DEBUG
        var random = new Random();
        Comparison<PlaylistItem> comparison = (_, _) => random.NextDouble() < 0.5 ? -1 : 1;
#else
        Comparison<PlaylistItem> comparison = (_, _) => Random.value < 0.5f ? -1 : 1;
#endif
        _items.Sort(comparison);
    }

    private bool Begin(PlaylistItem item, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        EndCurrent();
        State = PlaylistState.MovingToNextItem;
        try
        {
            var created = item.CreateProvider(WaveFormat.SampleRate, WaveFormat.Channels);
            provider = created.WaveFormat.Matches(WaveFormat)
                ? created
                : ProviderToProcessor.SampleProviderToProcessor(created, true)
                    .ToChain()
                    .ToFormat(WaveFormat.SampleRate, WaveFormat.Channels);
            _current = (item, provider);
            RepeatMode = RepeatMode;
            return true;
        }
        catch (Exception e)
        {
            // TODO: add log message, remove debug
#if DEBUG
            Console.WriteLine($"Failed to play {item}");
            Console.WriteLine(e);
#else
            Debug.LogError($"Failed to play {item}");
            Debug.LogError(e);
#endif
            provider = null;
            return false;
        }
    }

    private void EndCurrent()
    {
        (_current?.Provider as IDisposable)?.Dispose();
        _current = null;
    }

    private void End(bool wasPlaying)
    {
        State = PlaylistState.Ended;
        if (wasPlaying)
            LastItemEnded.InvokeSafely();
        EndCurrent();
    }

    private T? GetSource<T>() where T : notnull => _current.GetValueOrDefault().Provider switch
    {
        T t => t,
        IAudioProcessor processor when processor.TryGetSourceAs(out T? provider) => provider,
        _ => default
    };

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; }

    /// <summary>
    /// Clears the playlist, and disposes of the current provider. 
    /// </summary>
    public void Dispose()
    {
        EndCurrent();
        _items.Clear();
        State = PlaylistState.Ended;
        BeforeStarted = CurrentItemChanged = LastItemEnded = null;
    }

}
