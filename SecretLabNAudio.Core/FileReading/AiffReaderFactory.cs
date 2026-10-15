namespace SecretLabNAudio.Core.FileReading;

internal sealed class AiffReaderFactory : IAudioReaderFactory
{

    public IStreamAudioProcessor FromPath(string path) => new WaveStreamAudioProcessor(new AiffFileReader(path)) {FilePath = path};

    public IStreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => new WaveStreamAudioProcessor(new AiffFileReader(stream), closeOnDispose);

}
