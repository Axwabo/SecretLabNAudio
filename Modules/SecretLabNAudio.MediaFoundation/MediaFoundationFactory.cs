using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.MediaFoundation;

internal sealed class MediaFoundationFactory : IAudioReaderFactory
{

    public IStreamAudioProcessor FromPath(string path) => new WaveStreamAudioProcessor(new MediaFoundationReader(path)) {FilePath = path};

    public IStreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => new WaveStreamAudioProcessor(new StreamMediaFoundationReader(stream), closeOnDispose);

}
