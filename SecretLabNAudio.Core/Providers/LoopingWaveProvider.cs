using NAudio.Wave;

namespace SecretLabNAudio.Core.Providers;

public sealed class LoopingWaveProvider : IWaveProvider, IDisposable
{

    private readonly WaveStream _stream;

    public LoopingWaveProvider(WaveStream stream) => _stream = stream;

    public WaveFormat WaveFormat => _stream.WaveFormat;

    public int Read(byte[] buffer, int offset, int count)
    {
        var read = _stream.Read(buffer, offset, count);
        if (read == count)
            return count;
        _stream.Position = 0;
        read += _stream.Read(buffer, offset + read, count - read);
        return read;
    }

    public void Dispose() => _stream.Dispose();

}
