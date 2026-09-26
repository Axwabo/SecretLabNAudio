namespace SecretLabNAudio.Core.Encoded.OggOpus;

public readonly struct OggPage
{

    private readonly ReadOnlyMemory<byte> _rawPacket;

    public long GranulePosition { get; }

    public HeaderType Flags { get; }

    public ReadOnlyMemory<Range> Ranges { get; }

    public ReadOnlySpan<byte> this[int index] => _rawPacket.Span[Ranges.Span[index]];

    public int Count => Ranges.Length;

    public int TotalSize => _rawPacket.Length;

    public bool IsEndOfStream => (Flags & HeaderType.EndOfStream) != 0;

    public OggPage(long granulePosition, HeaderType flags, ReadOnlyMemory<byte> rawPacket, ReadOnlySpan<byte> packetLengths)
    {
        GranulePosition = granulePosition;
        Flags = flags;
        var total = 0;
        var ranges = new Range[packetLengths.Length];
        for (var i = 0; i < packetLengths.Length; i++)
        {
            var packetLength = packetLengths[i];
            var end = total + packetLength;
            ranges[i] = total..end;
            total = end;
        }

        Ranges = ranges;
        _rawPacket = rawPacket[..total];
    }

    private OggPage(long granulePosition, HeaderType flags, ReadOnlyMemory<byte> rawPacket, (ReadOnlyMemory<Range> Ranges, int Total) sum)
    {
        GranulePosition = granulePosition;
        Flags = flags;
        Ranges = sum.Ranges;
        _rawPacket = rawPacket[..sum.Total];
    }

    internal OggPage CopyTo(Memory<byte> destination)
    {
        if (destination.Length < TotalSize)
            throw new ArgumentException("Destination buffer is smaller than the current packet");
        _rawPacket.CopyTo(destination);
        return new OggPage(GranulePosition, Flags, destination, (Ranges, TotalSize));
    }

}
