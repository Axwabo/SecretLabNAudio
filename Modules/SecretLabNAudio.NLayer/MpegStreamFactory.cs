using NLayer.NAudioSupport;
using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.NLayer;

internal sealed class MpegStreamFactory : IAudioReaderFactory
{

    public StreamAudioProcessor FromPath(string path) => new(new ManagedMpegStream(path)) {FilePath = path};

    public StreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => new(new ManagedMpegStream(stream, closeOnDispose), closeOnDispose);

}
