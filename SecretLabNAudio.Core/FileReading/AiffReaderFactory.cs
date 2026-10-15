namespace SecretLabNAudio.Core.FileReading;

internal sealed class AiffReaderFactory : IAudioReaderFactory
{

    public IStreamAudioProcessor FromPath(string path) => new StreamAudioProcessor(new AiffFileReader(path)) {FilePath = path};

    public IStreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => new StreamAudioProcessor(new AiffFileReader(stream), closeOnDispose);

}
