using System.IO;
using NAudio.Wave;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.MediaFoundation;

internal sealed class MediaFoundationFactory : IAudioReaderFactory
{

    public AudioReaderFactoryResult FromPath(string path) => new MediaFoundationReader(path);

    public AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose) => new DisposableMediaFoundationReader(stream, closeOnDispose);

}

file sealed class DisposableMediaFoundationReader : StreamMediaFoundationReader
{

    private readonly ConditionalOneShotDisposable _disposable;

    public DisposableMediaFoundationReader(Stream stream, bool closeOnDispose) : base(stream, new MediaFoundationReaderSettings())
        => _disposable = new ConditionalOneShotDisposable(stream, closeOnDispose);

    protected override void Dispose(bool disposing)
    {
        _disposable.Dispose(disposing);
        base.Dispose(disposing);
    }

}
