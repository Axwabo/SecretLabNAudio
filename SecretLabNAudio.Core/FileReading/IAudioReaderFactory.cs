using System.IO;

namespace SecretLabNAudio.Core.FileReading;

public interface IAudioReaderFactory
{

    AudioReaderFactoryResult FromPath(string path);

    AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose);

}
