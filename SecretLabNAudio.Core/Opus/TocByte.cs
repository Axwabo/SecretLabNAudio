namespace SecretLabNAudio.Core.Opus;

public static class TocByte
{

    private const int CountMask = 0b11;
    private const int StereoMask = 0b1;
    private const int Size = AudioConstants.SampleRate;

    public static int FrameCount(ReadOnlySpan<byte> packet) => (FrameCountCode) (packet[0] & CountMask) switch
    {
        FrameCountCode.One => 1,
        FrameCountCode.TwoEqual or FrameCountCode.TwoDifferent => 2,
        FrameCountCode.Arbitrary => packet[1] & 0b111111,
        _ => throw new ArgumentException("Malformed packet: invalid TOC byte")
    };

    public static int SamplesPerFrame(byte toc)
    {
        if ((toc & 0x80) != 0)
            return (Size << (toc >> 3 & CountMask)) / 400;
        if ((toc & 0x60) == 0x60)
            return (toc & 0x08) != 0
                ? Size / 50
                : Size / 100;
        var power = toc >> 3 & CountMask;
        return power == 3
            ? Size * 60 / 1000
            : (Size << power) / 100;
    }

    public static int TotalSamples(ReadOnlySpan<byte> packet) => SamplesPerFrame(packet[0]) * FrameCount(packet);

    public static int TotalSamplesPerChannel(ReadOnlySpan<byte> packet)
    {
        var total = TotalSamples(packet);
        var stereo = (packet[0] >> 2 & StereoMask) != 0;
        return stereo ? total / 2 : total;
    }

}
