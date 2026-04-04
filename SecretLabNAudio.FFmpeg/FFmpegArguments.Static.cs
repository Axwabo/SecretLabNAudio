using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg;

public readonly partial record struct FFmpegArguments
{

    /// <summary>
    /// The timestamp format FFmpeg accepts that can be passed to <see cref="TimeSpan.ToString(string)">TimeSpan.ToString</see>.
    /// </summary>
    public const string TimestampFormat = @"hh\:mm\:ss\.fff";

    /// <summary>The standard pipe input/output.</summary>
    public const string StandardPipe = "-";

    /// <summary>32-bit float little endian format.</summary>
    public const string Float32Format = "f32le";

    private const string VerbosityError = "-v error ";

    private const string InputMissing = "Input must be specified";
    private const string InputHasQuotation = "Input must not include quotation marks";
    private const string OutputMissing = "Output must be specified";
    private const string OutputHasQuotation = "Output must not include quotation marks";
    private const string StdoutFormat = $"{VerbosityError}-i \"{{0}}\" -ar {{1}} -ac {{2}} -f {Float32Format} {StandardPipe}";

    /// <summary>
    /// A template for player-compatible reading from the standard output.
    /// </summary>
    /// <remarks>This does not include an input.</remarks>
    public static FFmpegArguments PlayerCompatibleStdout { get; } = new()
    {
        SampleRate = AudioPlayer.SampleRate,
        Channels = AudioPlayer.Channels,
        MuxerFormat = Float32Format,
        Output = StandardPipe
    };

    /// <summary>
    /// Builds a string to pass to FFmpeg that outputs 32-bit floats to the standard output.
    /// </summary>
    /// <param name="input">The input source (e.g. file path, URL).</param>
    /// <param name="sampleRate">The sample rate to output.</param>
    /// <param name="channels">The number of channels to output.</param>
    /// <returns>An arguments string to pass to FFmpeg.</returns>
    /// <include file='XmlDocs/Args.xml' path='doc/InArg/exception'/>
    public static string ToStdoutString(string input, int sampleRate, int channels) => string.Format(
        StdoutFormat,
        input.ValidateProcessArgument(nameof(input), InputMissing, InputHasQuotation),
        sampleRate,
        channels
    );

    /// <summary>
    /// Creates a new <see cref="FFmpegArguments"/> struct that reads from the standard input and outputs 32-bit floats to the standard output.
    /// </summary>
    /// <param name="sampleRate">The sample rate to output.</param>
    /// <param name="channels">The number of channels to output.</param>
    /// <returns>A new <see cref="FFmpegArguments"/> struct.</returns>
    public static FFmpegArguments StdinToStdout(int sampleRate, int channels)
        => new(false, null, StandardPipe, sampleRate, channels, null, Float32Format, StandardPipe);

    /// <summary>
    /// Creates an IEEEFloat <see cref="WaveFormat"/> based on the arguments' <see cref="SampleRate"/> and <see cref="Channels"/>.
    /// </summary>
    /// <param name="arguments">The arguments to convert.</param>
    /// <returns><see cref="AudioPlayer.SupportedFormat"/> if the arguments are player-compatible, otherwise, a new <see cref="WaveFormat"/>.</returns>
    public static implicit operator WaveFormat(FFmpegArguments arguments)
        => arguments is {SampleRate: AudioPlayer.SampleRate, Channels: AudioPlayer.Channels}
            ? AudioPlayer.SupportedFormat
            : WaveFormat.CreateIeeeFloatWaveFormat(arguments.SampleRate, arguments.Channels);

    /// <summary>
    /// Formats the specified <see cref="TimeSpan"/> to be acceptable by FFmpeg.
    /// </summary>
    /// <param name="timeSpan">The value to format.</param>
    /// <returns>A string representing the value.</returns>
    public static string Format(TimeSpan timeSpan) => timeSpan.ToString(TimestampFormat);

}
