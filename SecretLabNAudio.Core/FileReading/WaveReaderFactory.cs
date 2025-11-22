using System.IO;

namespace SecretLabNAudio.Core.FileReading;

internal sealed class WaveReaderFactory : IAudioReaderFactory
{

    public AudioReaderFactoryResult FromPath(string path) => new WaveFileReader(path);

    public AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose) => new DisposableWaveReader(stream, closeOnDispose);

}

file sealed class DisposableWaveReader : WaveFileReader
{

    private Stream? _stream;

    public DisposableWaveReader(Stream stream, bool closeOnDispose) : base(stream)
        => _stream = closeOnDispose ? stream : null;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;
        _stream?.Dispose();
        _stream = null;
    }

}
