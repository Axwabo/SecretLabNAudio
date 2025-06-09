using System.IO;
using NAudio.Vorbis;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.NVorbis;

internal sealed class VorbisStreamFactory : IAudioReaderFactory
{

    private static AudioReaderFactoryResult Result(VorbisWaveReader reader)
        => new(reader, reader);

    public AudioReaderFactoryResult FromPath(string path) => Result(new VorbisWaveReader(path));

    public AudioReaderFactoryResult FromStream(Stream stream, bool closeOnDispose) => Result(new VorbisWaveReader(stream, closeOnDispose));

}
