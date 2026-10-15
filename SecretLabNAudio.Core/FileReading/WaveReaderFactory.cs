namespace SecretLabNAudio.Core.FileReading;

internal sealed class WaveReaderFactory : IAudioReaderFactory
{

    public IStreamAudioProcessor FromPath(string path) => new StreamAudioProcessor(new WaveFileReader(path)) {FilePath = path};

    public IStreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => new StreamAudioProcessor(new WaveFileReader(stream), closeOnDispose);

}
