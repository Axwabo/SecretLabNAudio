namespace SecretLabNAudio.Core.Encoded;

public interface IOpusPacketProvider : IDisposable
{

    int ReadPacket(Span<byte> buffer);

}
