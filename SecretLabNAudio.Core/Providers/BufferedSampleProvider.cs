using VoiceChat.Networking;

namespace SecretLabNAudio.Core.Providers;

/// <summary>A sample provider that reads ahead by a given amount of samples (if available).</summary>
/// <seealso cref="PlaybackBufferSampleProvider"/>
/// <seealso cref="PlaybackBuffer"/>
public sealed class BufferedSampleProvider : ISampleProvider
{

    private static readonly float[] ReadBuffer = new float[AudioPlayer.SamplesPerPacket];

    private readonly ISampleProvider _source;
    private readonly PlaybackBuffer _buffer;
    private readonly int _size;

    /// <summary>Creates a new <see cref="BufferedSampleProvider"/>.</summary>
    /// <param name="source">The source sample provider to read from.</param>
    /// <param name="seconds">The number of seconds to read ahead.</param>
    public BufferedSampleProvider(ISampleProvider source, double seconds)
        : this(source, source.WaveFormat.SampleCount(seconds))
    {
    }

    /// <summary>Creates a new <see cref="BufferedSampleProvider"/>.</summary>
    /// <param name="source">The source sample provider to read from.</param>
    /// <param name="capacity">The number of samples to read ahead.</param>
    public BufferedSampleProvider(ISampleProvider source, int capacity)
    {
        _source = source;
        _size = capacity;
        _buffer = new PlaybackBuffer(capacity, true);
    }

    /// <inheritdoc/>
    public WaveFormat WaveFormat => _source.WaveFormat;

    /// <inheritdoc/>
    public int Read(float[] buffer, int offset, int count)
    {
        while (_buffer.Length < _size)
        {
            var read = _source.Read(ReadBuffer, 0, AudioPlayer.SamplesPerPacket);
            if (read == 0)
                break;
            _buffer.Write(ReadBuffer, read);
        }

        var target = Math.Min(count, _buffer.Length);
        if (target == 0)
            return 0;
        _buffer.ReadTo(buffer, target, offset);
        return target;
    }

    /// <summary>Clears the buffer.</summary>
    public void Clear() => _buffer.Clear();

    /// <summary>Disposes the underlying playback buffer when this object is finalized, allowing its buffer to be used later.</summary>
    ~BufferedSampleProvider() => _buffer.Dispose();

}
