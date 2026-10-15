using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.MediaFoundation;

internal sealed class MediaFoundationFactory : IAudioReaderFactory
{

    public IStreamAudioProcessor FromPath(string path) => new StreamAudioProcessor(new MediaFoundationReader(path)) {FilePath = path};

    public IStreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => new StreamAudioProcessor(new StreamMediaFoundationReader(stream), closeOnDispose);

}
