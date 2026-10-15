namespace SecretLabNAudio.Core.Opus;

public interface IOpusPacketProvider : IDisposable
{

    int ReadPacket(Span<byte> buffer);

}
