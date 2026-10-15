using NLayer.NAudioSupport;
using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.NLayer;

internal sealed class MpegStreamFactory : IAudioReaderFactory
{

    public IStreamAudioProcessor FromPath(string path) => new StreamAudioProcessor(new ManagedMpegStream(path)) {FilePath = path};

    public IStreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => new StreamAudioProcessor(new ManagedMpegStream(stream, closeOnDispose), closeOnDispose);

}
