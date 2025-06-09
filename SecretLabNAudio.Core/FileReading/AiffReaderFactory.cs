using System.IO;
using NAudio.Wave;

namespace SecretLabNAudio.Core.FileReading;

internal sealed class AiffReaderFactory : IAudioReaderFactory
{

    public AudioReaderFactoryResult FromPath(string path) => new AiffFileReader(path);

    public AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose) => new DisposableAiffReader(stream, closeOnDispose);

}

file sealed class DisposableAiffReader : AiffFileReader
{

    private readonly bool _closeOnDispose;
    private Stream? _stream;

    public DisposableAiffReader(Stream stream, bool closeOnDispose) : base(stream)
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
