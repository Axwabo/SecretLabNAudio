using NAudio.Vorbis;
using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.NVorbis;

internal sealed class VorbisStreamFactory : IAudioReaderFactory
{

    public IStreamAudioProcessor FromPath(string path)
    {
        var reader = new VorbisWaveReader(path);
        return new StreamAudioProcessor(reader, reader) {FilePath = path};
    }

    public IStreamAudioProcessor FromStream(Stream stream, bool closeOnDispose)
    {
        var reader = new VorbisWaveReader(stream, closeOnDispose);
        return new StreamAudioProcessor(reader, reader, closeOnDispose);
    }

}
