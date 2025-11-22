namespace SecretLabNAudio.Core.Processors;

public class StreamAudioProcessor : IAudioProcessor, ISeekable, ILoopable
{

    private ISampleProvider? _provider;

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

    public StreamAudioProcessor(WaveStream stream) : this(stream, stream.ToSampleProvider())
    {
    }

    public StreamAudioProcessor(WaveStream stream, ISampleProvider provider)
    {
        _provider = provider;
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
        if (_provider == null)
            return;
        Stream.Dispose();
        Stream = null!;
        _provider = null;
    }

}
