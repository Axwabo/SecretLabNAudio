using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.MediaFoundation;

internal sealed class MediaFoundationFactory : IAudioReaderFactory
{

    public AudioReaderFactoryResult FromPath(string path) => new MediaFoundationReader(path);

    public AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose) => new DisposableMediaFoundationReader(stream, closeOnDispose);

}

file sealed class DisposableMediaFoundationReader : StreamMediaFoundationReader
{

    private Stream? _stream;

    public DisposableMediaFoundationReader(Stream stream, bool closeOnDispose) : base(stream, new MediaFoundationReaderSettings())
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
