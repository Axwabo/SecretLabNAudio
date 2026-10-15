using NAudio.Vorbis;
using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.NVorbis;

internal sealed class VorbisStreamFactory : IAudioReaderFactory
{

    public StreamAudioProcessor FromPath(string path) => new(new VorbisWaveReader(path)) {FilePath = path};

    public StreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => new(new VorbisWaveReader(stream, closeOnDispose), closeOnDispose);

}
