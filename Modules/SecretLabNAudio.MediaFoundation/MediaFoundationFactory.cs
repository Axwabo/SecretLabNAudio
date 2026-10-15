using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.MediaFoundation;

internal sealed class MediaFoundationFactory : IAudioReaderFactory
{

    public StreamAudioProcessor FromPath(string path) => new(new MediaFoundationReader(path));

    public StreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => new(new StreamMediaFoundationReader(stream), closeOnDispose);

}
