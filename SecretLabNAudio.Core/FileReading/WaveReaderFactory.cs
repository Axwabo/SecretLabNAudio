using System.IO;
using NAudio.Wave;

namespace SecretLabNAudio.Core.FileReading;

internal sealed class WaveReaderFactory : IAudioReaderFactory
{

    public AudioReaderFactoryResult FromPath(string path) => new WaveFileReader(path);

    public AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose) => new DisposableWaveReader(stream, closeOnDispose);

}

file sealed class DisposableWaveReader : WaveFileReader
{

    private readonly bool _closeOnDispose;
    private Stream? _stream;

    public DisposableWaveReader(Stream stream, bool closeOnDispose) : base(stream)
    {
        _stream = stream;
        _closeOnDispose = closeOnDispose;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _stream != null)
        {
            if (_closeOnDispose)
                _stream.Dispose();
            _stream = null;
        }

        base.Dispose(disposing);
    }

}
