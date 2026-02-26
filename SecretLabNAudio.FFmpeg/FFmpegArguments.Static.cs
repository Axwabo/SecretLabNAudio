using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg;

public readonly partial record struct FFmpegArguments
{

    public const string StandardPipe = "-";

    public const string Float32Format = "f32le";

    private const string VerbosityError = "-v error ";

    private const string InputMissing = "Input must be specified";
    private const string InputHasQuotation = "Input must not include quotation marks";
    private const string OutputHasQuotation = "Output must not include quotation marks";
    private const string OutputMissing = "Output must be specified";
    private const string StdoutFormat = $"{VerbosityError}-i \"{{0}}\" -ar {{1}} -ac {{2}} -f {Float32Format} {StandardPipe}";

    public static FFmpegArguments PlayerCompatibleStdout { get; } = new()
    {
        SampleRate = AudioPlayer.SampleRate,
        Channels = AudioPlayer.Channels,
        MuxerFormat = Float32Format,
        Output = StandardPipe
    };

    public static string ToStdoutString(string input, int sampleRate, int channels) => string.Format(
        StdoutFormat,
        input.ValidateProcessArgument(nameof(input), InputMissing, InputHasQuotation),
        sampleRate,
        channels
    );

    public static FFmpegArguments StdinToStdout(int sampleRate, int channels)
        => new(false, null, StandardPipe, sampleRate, channels, null, Float32Format, StandardPipe);

    public static implicit operator WaveFormat(FFmpegArguments arguments)
        => arguments is {SampleRate: AudioPlayer.SampleRate, Channels: AudioPlayer.Channels}
            ? AudioPlayer.SupportedFormat
            : WaveFormat.CreateIeeeFloatWaveFormat(arguments.SampleRate, arguments.Channels);

}
