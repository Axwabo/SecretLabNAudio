using SecretLabNAudio.Core.Opus.Ogg;

namespace SecretLabNAudio.Core.FileReading;

internal sealed class OpusReaderFactory : IAudioReaderFactory
{

    public IStreamAudioProcessor? FromPath(string path)
    {
        var stream = File.OpenRead(path);
        return OggOpusPacketProvider.TryCreate(stream) is { } provider
            ? new OggOpusStreamProcessor(provider) {FilePath = path}
            : null;
    }

    public IStreamAudioProcessor FromStream(Stream stream, bool closeOnDispose) => throw new NotImplementedException();

}
