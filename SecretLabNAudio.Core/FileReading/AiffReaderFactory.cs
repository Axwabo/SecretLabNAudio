using System.IO;

namespace SecretLabNAudio.Core.FileReading;

internal sealed class AiffReaderFactory : IAudioReaderFactory
{

    public AudioReaderFactoryResult FromPath(string path) => new AiffFileReader(path);

    public AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose) => new DisposableAiffReader(stream, closeOnDispose);

}

file sealed class DisposableAiffReader : AiffFileReader
{

    private Stream? _stream;

    public DisposableAiffReader(Stream stream, bool closeOnDispose) : base(stream)
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
