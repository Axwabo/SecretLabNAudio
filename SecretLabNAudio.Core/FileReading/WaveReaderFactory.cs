namespace SecretLabNAudio.Core.FileReading;

internal sealed class WaveReaderFactory : IAudioReaderFactory
{

    public StreamAudioProcessor FromPath(string path) => new(new WaveFileReader(path)) {FilePath = path};

    public StreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => new(new WaveFileReader(stream), closeOnDispose);

}
