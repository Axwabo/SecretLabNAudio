using System.IO;
using NAudio.Wave;

namespace SecretLabNAudio.Core.FileReading;

public sealed class WaveReaderFactory : IAudioReaderFactory
{

    private static AudioReaderFactoryResult Result(WaveFileReader reader)
        => new(reader, reader.ToSampleProvider());

    public AudioReaderFactoryResult FromPath(string path) => Result(new WaveFileReader(path));

    public AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose) => Result(new WaveFileReader(stream));

}
