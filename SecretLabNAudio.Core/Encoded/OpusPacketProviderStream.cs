namespace SecretLabNAudio.Core.Encoded;

public sealed class OpusPacketProviderStream : IOpusPacketProvider
{

    private readonly Stream _source;

    public OpusPacketProviderStream(Stream source) => _source = source;

    public int ReadPacket(Span<byte> buffer)
    {
        var countBuffer = buffer[..2];
        if (_source.Read(countBuffer) != 2)
            return 0;
        var count = BitConverter.ToInt16(countBuffer);
        return count <= 0 ? 0 : _source.Read(buffer);
    }

    public void Dispose() => _source.Dispose();

}
