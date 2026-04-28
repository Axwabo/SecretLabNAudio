using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Extensions.Processors;
using Random = System.Random;

namespace SecretLabNAudio.Core.Processors.Playlists;

public sealed class LazyPlaylist : IAudioProcessor
{

    private readonly List<PlaylistItem> _items = [];

    private (PlaylistItem Item, ISampleProvider Provider)? _current;

    public int Index { get; private set; }

    public PlaylistState State { get; private set; }

    public IReadOnlyList<PlaylistItem> Items { get; }

    public bool ShuffleOnStart { get; set; }

    public Repeat RepeatMode { get; set; }

    public PlaylistItem? CurrentItem => _current?.Item;

    public event Action? CurrentItemChanged;

    public event Action? LastItemEnded;

    public LazyPlaylist(WaveFormat waveFormat)
    {
        WaveFormat = waveFormat;
        Items = _items.AsReadOnly();
    }

    public LazyPlaylist(WaveFormat waveFormat, params IEnumerable<PlaylistItem> items) : this(waveFormat)
    {
        _items.AddRange(items);
        Items = _items.AsReadOnly();
    }

    public LazyPlaylist Add(PlaylistItem item)
    {
        _items.Add(item);
        return this;
    }

    public int Read(float[] buffer, int offset, int count)
    {
        var total = 0;
        while (total < count && TryGetCurrent(out var provider))
        {
            var target = Math.Max(0, count - total);
            var read = provider.Read(buffer, total + offset, target);
            total += read;
            if (read < target)
                State = PlaylistState.BetweenItems;
        }

        return total;
    }

    private bool TryGetCurrent([NotNullWhen(true)] out ISampleProvider? provider)
    {
        switch (State, RepeatMode)
        {
            case (PlaylistState.NotStarted, _):
            case (PlaylistState.Ended, Repeat.All):
            case (PlaylistState.BetweenItems, Repeat.All) when Index >= _items.Count:
                return Restart(out provider);
            case (PlaylistState.BetweenItems, Repeat.One) when _current.HasValue:
                return Begin(_current.Value.Item, out provider);
            case (PlaylistState.BetweenItems, _):
                if (Index < _items.Count - 1)
                    return Next(true, out provider);
                End(true);
                provider = null;
                return false;
            case (PlaylistState.Ended, _):
                provider = null;
                return false;
            default:
                provider = _current?.Provider;
                return provider != null;
        }
    }

    private bool Restart([NotNullWhen(true)] out ISampleProvider? provider)
    {
        EndCurrent();
        _current = null;
        State = PlaylistState.Ended;
        if (_items.Count == 0)
        {
            provider = null;
            return false;
        }

        if (ShuffleOnStart)
            Shuffle();
        Index = 0;
        return Next(false, out provider);
    }

    private bool Next(bool advance, [NotNullWhen(true)] out ISampleProvider? provider)
    {
        var wasPlaying = State is PlaylistState.PlayingItem or PlaylistState.BetweenItems;
        if (advance && Index < _items.Count - 1)
            Index++;
        while (Index < _items.Count)
        {
            if (!Begin(_items[Index], out provider))
            {
                Index++;
                continue;
            }

            State = PlaylistState.PlayingItem;
            CurrentItemChanged.InvokeSafely();
            return true;
        }

        End(wasPlaying);
        provider = null;
        return false;
    }

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
        State = PlaylistState.BetweenItems;
        try
        {
            var created = item.CreateProvider(WaveFormat.SampleRate, WaveFormat.Channels);
            provider = created.WaveFormat.Matches(WaveFormat)
                ? created
                : ProviderToProcessor.SampleProviderToProcessor(created, true)
                    .ToChain()
                    .ToFormat(WaveFormat.SampleRate, WaveFormat.Channels);
            _current = (item, provider);
            return true;
        }
        catch (Exception e)
        {
            // TODO: add log message, remove debug
#if DEBUG
            Console.WriteLine(e);
#else
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

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; }

    public void Dispose()
    {
        EndCurrent();
        _current = null;
        State = PlaylistState.Ended;
        CurrentItemChanged = null;
    }

}
