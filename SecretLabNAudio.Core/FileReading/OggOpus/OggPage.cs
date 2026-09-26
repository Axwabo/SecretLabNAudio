namespace SecretLabNAudio.Core.FileReading.OggOpus;

public readonly ref struct OggPage
{

    private readonly ReadOnlySpan<byte> _rawPacket;

    public long GranulePosition { get; }

    public HeaderType Flags { get; }

    public ReadOnlySpan<Range> Ranges { get; }

    public ReadOnlySpan<byte> this[int index] => _rawPacket[Ranges[index]];

    public int Count => Ranges.Length;

    public int TotalSize => _rawPacket.Length;

    public bool IsValid => !_rawPacket.IsEmpty;

    public OggPage(long granulePosition, HeaderType flags, ReadOnlySpan<byte> rawPacket, ReadOnlySpan<byte> packetLengths)
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

}
