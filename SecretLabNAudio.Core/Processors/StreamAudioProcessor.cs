namespace SecretLabNAudio.Core.Processors;

/// <summary>A loopable, managed <see cref="IAudioProcessor"/> wrapping a <see cref="WaveStream"/>.</summary>
public sealed class StreamAudioProcessor : IAudioProcessor, ISeekable, ILoopable
{

    private ISampleProvider? _provider;
    private IDisposable? _disposable;

    /// <summary>The <see cref="WaveStream"/> that is encapsulated.</summary>
    /// <exception cref="ObjectDisposedException">Thrown if the processor has already been disposed.</exception>
    public WaveStream Stream
    {
        get
        {
            ThrowIfDisposed();
            return field;
        }
    }

    /// <summary>The path to the file if this processor was created from one.</summary>
    public string? FilePath { get; init; }

    /// <inheritdoc />
    /// <exception cref="ObjectDisposedException">Thrown if the processor has already been disposed.</exception>
    public TimeSpan CurrentTime
    {
        get => Stream.CurrentTime;
        set => Stream.CurrentTime = value;
    }

    /// <inheritdoc />
    public TimeSpan TotalTime => Stream.TotalTime;

    /// <inheritdoc />
    public bool Loop { get; set; }

    /// <summary>
    /// Creates a new <see cref="StreamAudioProcessor"/>, and automatically converts the stream to an <see cref="ISampleProvider"/>.
    /// </summary>
    /// <param name="stream">The <see cref="WaveStream"/> to encapsulate.</param>
    /// <param name="isOwned">Whether to dispose of the <paramref name="stream"/> when this object is disposed.</param>
    public StreamAudioProcessor(WaveStream stream, bool isOwned = true) : this(stream, stream.ToSampleProvider(), isOwned)
    {
    }

    /// <summary>
    /// Creates a new <see cref="StreamAudioProcessor"/> with a caller-provided <see cref="ISampleProvider"/>.
    /// </summary>
    /// <param name="stream">The <see cref="WaveStream"/> to encapsulate.</param>
    /// <param name="provider">The <see cref="ISampleProvider"/> corresponding to the <paramref name="stream"/>.</param>
    /// <param name="isOwned">Whether to dispose of the <paramref name="stream"/> when this object is disposed.</param>
    public StreamAudioProcessor(WaveStream stream, ISampleProvider provider, bool isOwned = true)
    {
        _provider = provider;
        _disposable = isOwned ? stream : null;
        Stream = stream;
    }

    /// <inheritdoc />
    /// <exception cref="ObjectDisposedException">Thrown if the processor has already been disposed.</exception>
    public WaveFormat WaveFormat
    {
        get
        {
            ThrowIfDisposed();
            return _provider!.WaveFormat;
        }
    }

    /// <inheritdoc />
    /// <exception cref="ObjectDisposedException">Thrown if the processor has already been disposed.</exception>
    public int Read(float[] buffer, int offset, int count)
    {
        ThrowIfDisposed();
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

    private void ThrowIfDisposed()
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
