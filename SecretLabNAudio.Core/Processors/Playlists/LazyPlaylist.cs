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
        while (total < count)
        {
            if (State is PlaylistState.NotStarted or PlaylistState.Ended && !Restart()
                || State == PlaylistState.BetweenItems && !Next()
                || !_current.HasValue)
                break;
            var target = Math.Max(0, count - total);
            var read = _current.GetValueOrDefault().Provider.Read(buffer, total + offset, target);
            total += read;
            if (read < target)
                State = PlaylistState.BetweenItems;
        }

        return total;
    }

    private bool Restart()
    {
        EndCurrent();
        _current = null;
        State = PlaylistState.Ended;
        if (_items.Count == 0 || RepeatMode == Repeat.None && State != PlaylistState.NotStarted)
            return false;
        if (ShuffleOnStart)
            Shuffle();
        Index = 0;
        return Next();
    }

    private bool Next()
    {
        var wasPlaying = State is PlaylistState.PlayingItem or PlaylistState.BetweenItems;
        if (wasPlaying && Index < _items.Count - 1)
            Index++;
        while (Index < _items.Count)
        {
            if (!BeginCurrent())
            {
                Index++;
                continue;
            }

            State = PlaylistState.PlayingItem;
            CurrentItemChanged.InvokeSafely();
            return true;
        }

        State = PlaylistState.Ended;
        EndCurrent();
        if (wasPlaying)
            LastItemEnded.InvokeSafely();
        _current = null;
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

    private bool BeginCurrent()
    {
        try
        {
            var item = _items[Index];
            var created = item.CreateProvider(WaveFormat.SampleRate, WaveFormat.Channels);
            var final = created.WaveFormat.Matches(WaveFormat)
                ? created
                : ProviderToProcessor.SampleProviderToProcessor(created, true)
                    .ToChain()
                    .ToFormat(WaveFormat.SampleRate, WaveFormat.Channels);
            _current = (item, final);
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
            return false;
        }
    }

    private void EndCurrent()
    {
        State = PlaylistState.BetweenItems;
        (_current?.Provider as IDisposable)?.Dispose();
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
