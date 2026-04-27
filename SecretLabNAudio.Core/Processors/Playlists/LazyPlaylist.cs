namespace SecretLabNAudio.Core.Processors.Playlists;

public sealed class LazyPlaylist : IAudioProcessor
{

    private readonly List<PlaylistItem> _items = [];

    public LazyPlaylist(WaveFormat waveFormat) => WaveFormat = waveFormat;

    public LazyPlaylist(WaveFormat waveFormat, params IEnumerable<PlaylistItem> items)
    {
        WaveFormat = waveFormat;
        _items.AddRange(items);
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

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; }

    public void Dispose()
    {
    }

}
