using System.Text;

namespace SecretLabNAudio.Core.Encoded.OggOpus;

public static class PageReader
{

    private const int Magic = 4;
    private const int Header = 27;
    private const int HeaderRemaining = Header - Magic;

    private static readonly byte[] OggS = Encoding.ASCII.GetBytes(nameof(OggS));
    private static readonly byte[] OpusHead = Encoding.ASCII.GetBytes(nameof(OpusHead));
    private static readonly byte[] OpusTags = Encoding.ASCII.GetBytes(nameof(OpusTags));

    public static bool IsIDHeader(OggPage? page) => page is [{Length: >= 19} packet] && packet[..OpusHead.Length].SequenceEqual(OpusHead);

    public static bool IsCommentHeader(OggPage? page) => page is [{Length: >= 8} packet, ..] && packet[..OpusTags.Length].SequenceEqual(OpusTags);

    public static OggPage? Read(Stream stream, byte[] buffer)
    {
        var seekable = stream.CanSeek;
        var streamLength = seekable ? stream.Length : 0;
        var fourSpan = buffer.AsSpan(0, Magic);
        do
        {
            var read = stream.Read(fourSpan);
            if (read == Magic && fourSpan.SequenceEqual(OggS))
                return ReadReady(stream, buffer);
        }
        while (seekable && stream.Position < streamLength);

        return null;
    }

    private static OggPage? ReadReady(Stream stream, byte[] buffer)
    {
        if (stream.Read(buffer.AsSpan(Magic, HeaderRemaining)) != HeaderRemaining)
            return null;
        var flags = buffer[5];
        var granulePosition = BitConverter.ToInt32(buffer, 6);
        var packetCount = buffer[Header - 1];
        var packetSizes = buffer.AsSpan(0, packetCount);
        stream.ReadExactly(packetSizes, "Lacing values missing");
        var page = new OggPage(granulePosition, (HeaderType) flags, buffer, packetSizes);
        stream.ReadExactly(buffer.AsSpan(0, page.TotalSize), "Not enough data to fill packet buffer");
        return page;
    }

    extension(Stream stream)
    {

        private void ReadExactly(Span<byte> buffer, string exceptionMessage)
        {
            int start;
            int num;
            for (start = 0; start < buffer.Length; start += num)
            {
                num = stream.Read(buffer[start..]);
                if (num == 0)
                    throw new EndOfStreamException(exceptionMessage);
            }
        }

    }

}
