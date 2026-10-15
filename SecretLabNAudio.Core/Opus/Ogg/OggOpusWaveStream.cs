using VoiceChat.Codec;

namespace SecretLabNAudio.Core.Opus.Ogg;

public sealed class OggOpusWaveStream : WaveStream, IAudioProcessor
{

    private readonly OpusDecoder _decoder = new();

    public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    public int Read(float[] buffer, int offset, int count) => throw new NotImplementedException();

    public override WaveFormat WaveFormat { get; }
    public override long Length { get; }
    public override long Position { get; set; }

    protected override void Dispose(bool disposing)
    {
        _decoder.Dispose();
    }

}
