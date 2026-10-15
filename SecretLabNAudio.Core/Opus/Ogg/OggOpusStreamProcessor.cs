using VoiceChat.Codec;

namespace SecretLabNAudio.Core.Opus.Ogg;

public sealed class OggOpusStreamProcessor : IStreamAudioProcessor
{

    private readonly OpusDecoder _decoder = new();

    public OggOpusStreamProcessor(OggOpusPacketProvider provider)
    {
    }

    public void Dispose() => _decoder.Dispose();

    public int Read(float[] buffer, int offset, int count) => throw new NotImplementedException();

    public WaveFormat WaveFormat { get; }

    public TimeSpan CurrentTime { get; set; }

    public TimeSpan TotalTime { get; }

    public bool Loop { get; set; }

    public string? FilePath { get; init; }

}
