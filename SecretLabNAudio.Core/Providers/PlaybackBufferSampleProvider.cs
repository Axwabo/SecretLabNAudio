using VoiceChat.Networking;

namespace SecretLabNAudio.Core.Providers;

/// <summary>A sample provider that is backed by a <see cref="PlaybackBuffer"/> with write access.</summary>
public sealed class PlaybackBufferSampleProvider : ISampleProvider
{

    private readonly PlaybackBuffer _buffer;

    /// <summary>The number of samples in the buffer.</summary>
    public int Length => _buffer.Length;

    /// <summary>
    /// Whether to pad out the read buffer with zeroes if more samples are requested than available.
    /// If set to true, makes the provider never-ending.
    /// </summary>
    public bool ReadFully { get; set; }

    /// <summary>Creates a new endless <see cref="PlaybackBufferSampleProvider"/>.</summary>
    /// <param name="capacitySeconds">The capacity of the buffer in seconds.</param>
    /// <param name="sampleRate">The sample rate of the audio.</param>
    /// <param name="channels">The number of audio channels.</param>
    public PlaybackBufferSampleProvider(double capacitySeconds, int sampleRate, int channels = 1)
        : this(capacitySeconds, true, sampleRate, channels)
    {
    }

    /// <summary>Creates a new <see cref="PlaybackBufferSampleProvider"/>.</summary>
    /// <param name="capacitySeconds">The capacity of the buffer in seconds.</param>
    /// <param name="endless">Whether the buffer should be endless. If false, the buffer is cleared when more data is written than its capacity.</param>
    /// <param name="sampleRate">The sample rate of the audio.</param>
    /// <param name="channels">The number of audio channels.</param>
    public PlaybackBufferSampleProvider(double capacitySeconds, bool endless, int sampleRate, int channels = 1)
        : this(WaveFormatExtensions.SampleCount(capacitySeconds, sampleRate, channels), endless, sampleRate, channels)
    {
    }

    /// <summary>Creates a new <see cref="PlaybackBufferSampleProvider"/>.</summary>
    /// <param name="capacity">The capacity of the buffer in samples.</param>
    /// <param name="endless">Whether the buffer should be endless. If false, the buffer is cleared when more data is written than its capacity.</param>
    /// <param name="sampleRate">The sample rate of the audio.</param>
    /// <param name="channels">The number of audio channels.</param>
    public PlaybackBufferSampleProvider(int capacity, bool endless, int sampleRate, int channels = 1)
    {
        _buffer = new PlaybackBuffer(capacity, endless);
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
    }

    /// <inheritdoc />
    public WaveFormat WaveFormat { get; }

    /// <inheritdoc />
    public int Read(float[] buffer, int offset, int count)
    {
        var max = Math.Min(count, Length);
        _buffer.ReadTo(buffer, max, offset);
        if (!ReadFully)
            return max;
        Array.Clear(buffer, max, count - max);
        return count;
    }

    /// <summary>Writes all samples from the array to the playback buffer.</summary>
    /// <param name="buffer">The array of samples to write.</param>
    public void Write(float[] buffer) => _buffer.Write(buffer, buffer.Length);

    /// <summary>Writes a portion of the array to the playback buffer.</summary>
    /// <param name="buffer">The array containing samples to write.</param>
    /// <param name="offset">The offset in the array to start writing from.</param>
    /// <param name="count">The number of samples to write from the array.</param>
    public void Write(float[] buffer, int offset, int count) => _buffer.Write(buffer, count, offset);

    /// <summary>Writes a single sample to the playback buffer.</summary>
    /// <param name="sample">The sample to write.</param>
    public void Write(float sample) => _buffer.Write(sample);

    /// <summary>Clears the playback buffer.</summary>
    public void Clear() => _buffer.Clear();

    /// <summary>Disposes the underlying playback buffer when this object is finalized, allowing its buffer to be used later.</summary>
    ~PlaybackBufferSampleProvider() => _buffer.Dispose();

}
