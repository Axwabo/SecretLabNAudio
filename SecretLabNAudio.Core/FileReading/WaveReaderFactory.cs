namespace SecretLabNAudio.Core.FileReading;

internal sealed class WaveReaderFactory : IAudioReaderFactory
{

    public IStreamAudioProcessor FromPath(string path) => new WaveStreamAudioProcessor(new WaveFileReader(path)) {FilePath = path};

    public IStreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => new WaveStreamAudioProcessor(new WaveFileReader(stream), closeOnDispose);

}
