namespace SecretLabNAudio.Core.Processors;

public sealed class StreamAudioProcessor : IAudioProcessor, ISeekable, ILoopable
{

    private ISampleProvider? _provider;
    private IDisposable? _disposable;

    public WaveStream Stream
    {
        get
        {
            EnsureNotDisposed();
            return field;
        }
        private set;
    }

    /// <inheritdoc />
    public TimeSpan CurrentTime
    {
        get => Stream.CurrentTime;
        set => Stream.CurrentTime = value;
    }

    /// <inheritdoc />
    public TimeSpan TotalTime => Stream.TotalTime;

    /// <inheritdoc />
    public bool Loop { get; set; }

    public StreamAudioProcessor(WaveStream stream, bool isOwned = true) : this(stream, stream.ToSampleProvider(), isOwned)
    {
    }

    public StreamAudioProcessor(WaveStream stream, ISampleProvider provider, bool isOwned = true)
    {
        _provider = provider;
        _disposable = isOwned ? stream : null;
        Stream = stream;
    }

    /// <inheritdoc />
    public WaveFormat WaveFormat
    {
        get
        {
            EnsureNotDisposed();
            return _provider!.WaveFormat;
        }
    }

    /// <inheritdoc />
    public int Read(float[] buffer, int offset, int count)
    {
        EnsureNotDisposed();
        if (!Loop)
            return _provider!.Read(buffer, offset, count);
        var total = 0;
        while (total < count)
        {
            var target = count - total;
            var read = _provider!.Read(buffer, offset + total, target);
            if (read < target)
                Stream.Position = 0;
            total += read;
        }

        return total;
    }

    private void EnsureNotDisposed()
    {
        if (_provider == null)
            throw new ObjectDisposedException(nameof(StreamAudioProcessor));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _disposable?.Dispose();
        _disposable = null;
        _provider = null;
    }

}
