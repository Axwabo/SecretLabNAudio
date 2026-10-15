namespace SecretLabNAudio.Core.Opus;

public readonly record struct TocByte(byte Config, bool Stereo, FrameCountCode CountCode)
{

    private const int ConfigMask = 0b11111;
    private const int StereoMask = 0b1;
    private const int CountMask = 0b11;

    // https://datatracker.ietf.org/doc/html/rfc6716#section-3.1
    private static readonly int[] FrameSizeLut =
    [
        480, 960, 1920, 2880, // SILK NB
        480, 960, 1920, 2880, // SILK MB
        480, 960, 1920, 2880, // SILK WB
        480, 960, // Hybrid SWB
        480, 960, // Hybrid FB
        120, 240, 480, 960, // CELT NB
        120, 240, 480, 960, // CELT WB
        120, 240, 480, 960, // CELT SWB
        120, 240, 480, 960 // CELT FB
    ];

    public int FrameSize => FrameSizeLut[Config];

    public int GetFrameCount(ReadOnlySpan<byte> packet) => CountCode switch
    {
        FrameCountCode.One => 1,
        FrameCountCode.TwoEqual or FrameCountCode.TwoDifferent => 2,
        FrameCountCode.Arbitrary => packet[1] & 0b111111,
        _ => throw new InvalidOperationException()
    };

    public static implicit operator TocByte(byte toc) => new(
        (byte) (toc >> 3 & ConfigMask),
        (toc >> 2 & StereoMask) != 0,
        (FrameCountCode) (toc & CountMask)
    );

}
