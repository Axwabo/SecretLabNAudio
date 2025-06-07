using NAudio.Wave;
using VoiceChat.Networking;

namespace SecretLabNAudio.Core.Providers;

public sealed class BufferedSampleProvider : ISampleProvider
{

    private static readonly float[] ReadBuffer = new float[AudioPlayer.PacketSamples];

    private readonly ISampleProvider _provider;
    private readonly PlaybackBuffer _buffer;
    private readonly int _size;

    public BufferedSampleProvider(ISampleProvider provider, double seconds)
        : this(provider, (int) (provider.WaveFormat.SampleRate * provider.WaveFormat.Channels * seconds))
    {
    }

    public BufferedSampleProvider(ISampleProvider provider, int capacity)
    {
        _provider = provider;
        _size = capacity;
        _buffer = new PlaybackBuffer(capacity, true);
    }

    public WaveFormat WaveFormat => _provider.WaveFormat;

    public int Read(float[] buffer, int offset, int count)
    {
        while (_buffer.Length < _size)
        {
            var read = _provider.Read(ReadBuffer, 0, AudioPlayer.PacketSamples);
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

    public void Clear() => _buffer.Clear();

    ~BufferedSampleProvider() => _buffer.Dispose();

}
