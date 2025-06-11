using System.IO;

namespace SecretLabNAudio.Core.FileReading;

internal sealed class AiffReaderFactory : IAudioReaderFactory
{

    public AudioReaderFactoryResult FromPath(string path) => new AiffFileReader(path);

    public AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose) => new DisposableAiffReader(stream, closeOnDispose);

}

file sealed class DisposableAiffReader : AiffFileReader
{

    private readonly ConditionalOneShotDisposable _disposable;

    public DisposableAiffReader(Stream stream, bool closeOnDispose) : base(stream)
        => _disposable = new ConditionalOneShotDisposable(stream, closeOnDispose);

    protected override void Dispose(bool disposing)
    {
        _disposable.Dispose(disposing);
        base.Dispose(disposing);
    }

}
