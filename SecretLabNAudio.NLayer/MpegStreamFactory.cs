using System.IO;
using NLayer.NAudioSupport;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.NLayer;

internal sealed class MpegStreamFactory : IAudioReaderFactory
{

    public AudioReaderFactoryResult FromPath(string path) => new ManagedMpegStream(path);

    public AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose) => new ManagedMpegStream(stream, closeOnDispose);

}
