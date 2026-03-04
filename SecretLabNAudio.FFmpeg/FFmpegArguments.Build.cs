using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg;

public readonly partial record struct FFmpegArguments
{

    /// <summary>
    /// Returns a copy of this instance with <see cref="SampleRate"/> = <see cref="AudioPlayer.SampleRate"/> and <see cref="Channels"/> = <see cref="AudioPlayer.Channels"/>.
    /// </summary>
    /// <returns>A <see cref="AudioPlayer.SupportedFormat">player-compatible</see> copy with other properties preserved.</returns>
    public FFmpegArguments ToPlayerCompatible() => this with
    {
        SampleRate = AudioPlayer.SampleRate,
        Channels = AudioPlayer.Channels
    };

    /// <summary>
    /// Returns a copy of this instance that is suitable for reading 32-bit floats from the standard output.<br/>
    /// <see cref="ShowLogs"/> = false, <see cref="MuxerFormat"/> = <see cref="Float32Format"/> and <see cref="Output"/> = <see cref="StandardPipe"/>
    /// </summary>
    /// <returns>A 32-bit float stdout-compatible copy with other properties preserved.</returns>
    public FFmpegArguments ForFloatPiping() => this with
    {
        ShowLogs = false,
        MuxerFormat = Float32Format,
        Output = StandardPipe
    };

    public FFmpegArguments ForPlayerCompatibleFloatPiping() => new(false, InputOptions, Input, AudioPlayer.SampleRate, AudioPlayer.Channels, OutputOptions, Float32Format, StandardPipe);

    /// <summary>
    /// Returns a copy of this instance with <see cref="Input"/> = <see cref="StandardPipe"/>.
    /// </summary>
    /// <returns>A stdin-reading copy with other properties preserved.</returns>
    public FFmpegArguments ReadFromStandardInput() => this with {Input = StandardPipe};

    public FFmpegArguments PipeToStandardOutput() => this with {Output = StandardPipe};

    public FFmpegArguments WithInput(string input) => this with {Input = input.ValidateProcessArgument(nameof(input), InputMissing, InputHasQuotation)};

    public FFmpegArguments WithOutput(string output) => this with {Output = output.ValidateProcessArgument(nameof(output), OutputMissing, OutputHasQuotation)};

    public FFmpegArguments WithSampleRate(int sampleRate) => this with {SampleRate = sampleRate};

    public FFmpegArguments WithChannels(int channels) => this with {Channels = channels};

    public FFmpegArguments WithVolumeScalar(double scalar) => this with {OutputOptions = $"-af volume={scalar}"};

    public FFmpegArguments WithVolumeDecibels(double relativeDecibels) => this with {OutputOptions = $"-af volume={relativeDecibels}dB"};

    public FFmpegArguments WithComplexFilter(string filter) => this with {OutputOptions = $"-filter_complex \"{filter}\""};

    public FFmpegArguments EnableVerboseLogging() => this with {ShowLogs = true};

    public FFmpegArguments WithInputOptions(string? inputOptions) => this with {InputOptions = inputOptions};

    public FFmpegArguments WithOutputOptions(string? outputOptions) => this with {OutputOptions = outputOptions};

    public FFmpegArguments WithMuxerFormat(string? muxerFormat) => this with {MuxerFormat = muxerFormat};

    /// <summary>
    /// Sets the <see cref="InputOptions"/> loop the stream infinitely.
    /// </summary>
    /// <returns>An infinitely looping copy with properties except <see cref="InputOptions"/> preserved.</returns>
    public FFmpegArguments WithInfiniteLoop() => this with {InputOptions = "-stream_loop -1"};

    /// <summary>
    /// Sets the <see cref="InputOptions"/> loop the stream a given number of times. The stream will be played a total of <paramref name="count"/> + 1 times.
    /// </summary>
    /// <param name="count">The number of times to restart the stream.</param>
    /// <returns>A looping copy with properties except <see cref="InputOptions"/> preserved.</returns>
    public FFmpegArguments WithLoopCount(int count) => this with {InputOptions = $"-stream_loop {count}"};

    public FFmpegArguments WithStreamStart(TimeSpan startTime) => this with {InputOptions = $"-ss {Format(startTime)}"};

    public FFmpegArguments WithStreamEnd(TimeSpan endTime) => this with {InputOptions = $"-to {Format(endTime)}"};

    public FFmpegArguments WithStreamRange(TimeSpan startTime, TimeSpan endTime) => this with {InputOptions = $"-ss {Format(startTime)} -to {Format(endTime)}"};

}
