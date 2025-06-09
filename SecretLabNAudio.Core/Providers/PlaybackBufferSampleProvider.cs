using NAudio.Wave;
using SecretLabNAudio.Core.Extensions;
using VoiceChat.Networking;

namespace SecretLabNAudio.Core.Providers;

public sealed class PlaybackBufferSampleProvider : ISampleProvider
{

    private readonly PlaybackBuffer _buffer;

    public int Length => _buffer.Length;

    public bool ReadFully { get; set; }

    public PlaybackBufferSampleProvider(double capacitySeconds, bool endless, int sampleRate, int channels = 1)
        : this(WaveFormatExtensions.SampleCount(capacitySeconds, sampleRate, channels), endless, sampleRate, channels)
    {
    }

    public PlaybackBufferSampleProvider(int capacity, bool endless, int sampleRate, int channels = 1)
    {
        _buffer = new PlaybackBuffer(capacity, endless);
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
    }

    public WaveFormat WaveFormat { get; }

    public int Read(float[] buffer, int offset, int count)
    {
        var max = Math.Min(count, Length);
        _buffer.ReadTo(buffer, max, offset);
        if (!ReadFully)
            return max;
        Array.Clear(buffer, max, count - max);
        return count;
    }

    public void Write(float[] buffer) => _buffer.Write(buffer, buffer.Length);

    public void Write(float[] buffer, int offset, int length) => _buffer.Write(buffer, length, offset);

    public void Clear() => _buffer.Clear();

    ~PlaybackBufferSampleProvider() => _buffer.Dispose();

}
