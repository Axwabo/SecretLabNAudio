using Random = UnityEngine.Random;

namespace SecretLabNAudio.Core.Processors.Playlists;

public sealed class LazyPlaylist : IAudioProcessor
{

    private readonly List<PlaylistItem> _items = [];

    private PlaylistState _state;

    private (PlaylistItem Item, ISampleProvider Provider)? _current;

    public IReadOnlyList<PlaylistItem> Items { get; }

    public int Index { get; private set; }

    public bool ShuffleOnStart { get; set; }

    public LazyPlaylist(WaveFormat waveFormat) => WaveFormat = waveFormat;

    public LazyPlaylist(WaveFormat waveFormat, params IEnumerable<PlaylistItem> items)
    {
        WaveFormat = waveFormat;
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
        // TODO
        return 0;
    }

    private void Shuffle()
    {
#if DEBUG
        var random = new System.Random();
        Comparison<PlaylistItem> comparison = (_, _) => random.NextDouble() < 0.5 ? -1 : 1;
#else
        Comparison<PlaylistItem> comparison = (_, _) => Random.value < 0.5f ? -1 : 1;
#endif
        _items.Sort(comparison);
    }

    private void Begin()
    {
        if (Index < _items.Count)
            return;
        try
        {
            _current = (_items[Index], _items[Index].CreateProvider(WaveFormat.SampleRate, WaveFormat.Channels)); // TODO: convert
        }
        catch (Exception e)
        {
            // TODO: add log message, remove debug
#if DEBUG
            Console.WriteLine(e);
#else
            Debug.LogError(e);
#endif
            throw;
        }
    }

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; }

    public void Dispose()
    {
    }

}
