using VoiceChat;

namespace SecretLabNAudio.Core.Encoded.OggOpus;

public sealed class OpusPacketProviderStream : IOpusPacketProvider
{

    // TODO: thread safety
    private static readonly byte[] PageBuffer = new byte[ushort.MaxValue];

    private readonly Stream _source;

    private readonly long _dataStartPosition;

    private readonly byte[] _packetBuffer = new byte[VoiceChatSettings.MaxEncodedSize];

    public static OpusPacketProviderStream? TryCreate(Stream source)
    {
        var buffer = PageBuffer;
        if (!PageReader.Read(source, buffer).IsIDHeader)
            return null;
        var channels = buffer[9];
        if (!PageReader.Read(source, buffer).IsCommentHeader)
            return null;
        long position;
        OggPage? next;
        do
        {
            position = source.Position;
            next = PageReader.Read(source, buffer);
        }
        while (next.IsContinuation);

        if (next == null)
            return null;
        return new OpusPacketProviderStream(source, channels, position);
    }

    public int Channels { get; }

    private OpusPacketProviderStream(Stream source, int channels, long dataStartPosition)
    {
        _source = source;
        _dataStartPosition = dataStartPosition;
        Channels = channels;
    }

    public int ReadPacket(Span<byte> buffer) => throw new NotImplementedException();

    public void Dispose() => _source.Dispose();

}
