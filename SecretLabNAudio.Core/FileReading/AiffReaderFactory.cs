namespace SecretLabNAudio.Core.FileReading;

internal sealed class AiffReaderFactory : IAudioReaderFactory
{

    public StreamAudioProcessor FromPath(string path) => new(new AiffFileReader(path)) {FilePath = path};

    public StreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => new(new AiffFileReader(stream), closeOnDispose);

}
