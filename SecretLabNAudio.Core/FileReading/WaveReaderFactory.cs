using System.IO;

namespace SecretLabNAudio.Core.FileReading;

internal sealed class WaveReaderFactory : IAudioReaderFactory
{

    public AudioReaderFactoryResult FromPath(string path) => new WaveFileReader(path);

    public AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose) => new DisposableWaveReader(stream, closeOnDispose);

}

file sealed class DisposableWaveReader : WaveFileReader
{

    private readonly ConditionalOneShotDisposable _disposable;

    public DisposableWaveReader(Stream stream, bool closeOnDispose) : base(stream)
        => _disposable = new ConditionalOneShotDisposable(stream, closeOnDispose);

    protected override void Dispose(bool disposing)
    {
        _disposable.Dispose(disposing);
        base.Dispose(disposing);
    }

}
