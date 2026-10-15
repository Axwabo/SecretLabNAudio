using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Opus.Ogg;

namespace SecretLabNAudio.Core.Opus;

internal sealed class OpusReaderFactory : IAudioReaderFactory
{

    public IStreamAudioProcessor FromPath(string path)
    {
        var stream = File.OpenRead(path);
        return OggOpusPacketProvider.TryCreate(stream) is { } provider
            ? new WaveStreamAudioProcessor(new OggOpusWaveStream()) {FilePath = path}
            : throw new IOException("Not an Ogg Opus file");
    }

    public IStreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => throw new NotImplementedException();

}
