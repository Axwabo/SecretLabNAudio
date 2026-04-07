using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg;

public readonly partial record struct FFmpegArguments
{

    /// <summary>
    /// Returns a copy of this instance with <see cref="SampleRate"/> = <see cref="AudioConstants.SampleRate"/> and <see cref="Channels"/> = <see cref="AudioConstants.Channels"/>.
    /// </summary>
    /// <returns>A <see cref="AudioConstants.SupportedFormat">player-compatible</see> copy with other properties preserved.</returns>
    public FFmpegArguments ToPlayerCompatible() => this with
    {
        SampleRate = AudioConstants.SampleRate,
        Channels = AudioConstants.Channels
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

    /// <summary>
    /// Returns a copy of this instance that is suitable for reading 32-bit player-compatible floats from the standard output.
    /// This method is a combination of <see cref="ToPlayerCompatible"/> and <see cref="ForFloatPiping"/>.
    /// </summary>
    /// <returns>
    /// A 32-bit float stdout-compatible and <see cref="AudioConstants.SupportedFormat">player-compatible</see> instance
    /// with <see cref="InputOptions"/>, <see cref="Input"/> and <see cref="OutputOptions"/> preserved.
    /// </returns>
    public FFmpegArguments ForPlayerCompatibleFloatPiping() => new(false, InputOptions, Input, AudioConstants.SampleRate, AudioConstants.Channels, OutputOptions, Float32Format, StandardPipe);

    /// <summary>
    /// Returns a copy of this instance with <see cref="Input"/> = <see cref="StandardPipe"/>.
    /// </summary>
    /// <returns>A stdin-reading copy with other properties preserved.</returns>
    public FFmpegArguments ReadFromStandardInput() => this with {Input = StandardPipe};

    /// <summary>
    /// Returns a copy of this instance with <see cref="Output"/> = <see cref="StandardPipe"/>.
    /// </summary>
    /// <returns>A stdout-outputting copy with other properties preserved.</returns>
    public FFmpegArguments PipeToStandardOutput() => this with {Output = StandardPipe};

    /// <summary>
    /// Returns a copy of this instance with <see cref="Input"/> = <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The input source (e.g. file path, URL).</param>
    /// <returns>A copy of this instance with the new input. Other properties are preserved.</returns>
    /// <include file='XmlDocs/Args.xml' path='doc/InArg/exception'/>
    public FFmpegArguments WithInput(string input) => this with {Input = input.ValidateProcessArgument(nameof(input), InputMissing, InputHasQuotation)};

    /// <summary>
    /// Returns a copy of this instance with <see cref="Output"/> = <paramref name="output"/>.
    /// </summary>
    /// <param name="output">The destination to write to (e.g. file path, pipe).</param>
    /// <returns>A copy of this instance with the new output. Other properties are preserved.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the output matches any of the following:
    /// <list type="bullet">
    /// <item><description><see cref="string.IsNullOrWhiteSpace">null or whitespace</see></description></item>
    /// <item><description>contains a quotation mark (<c>&quot;</c>)</description></item>
    /// </list>
    /// </exception>
    public FFmpegArguments WithOutput(string output) => this with {Output = output.ValidateProcessArgument(nameof(output), OutputMissing, OutputHasQuotation)};

    /// <summary>
    /// Returns a copy of this instance with <see cref="SampleRate"/> = <paramref name="sampleRate"/>.
    /// </summary>
    /// <param name="sampleRate">The sample rate to output.</param>
    /// <returns>A copy of this instance with the new sample rate. Other properties are preserved.</returns>
    public FFmpegArguments WithSampleRate(int sampleRate) => this with {SampleRate = sampleRate};

    /// <summary>
    /// Returns a copy of this instance with <see cref="Channels"/> = <paramref name="channels"/>.
    /// </summary>
    /// <param name="channels">The number of channels to output.</param>
    /// <returns>A copy of this instance with the new channel count. Other properties are preserved.</returns>
    public FFmpegArguments WithChannels(int channels) => this with {Channels = channels};

    /// <summary>
    /// Returns a copy of this instance with the <see cref="OutputOptions"/> set to an audio filter that multiplies the samples by a scalar.
    /// </summary>
    /// <param name="scalar">The volume scalar. 1 = original volume, 0.5 = 50% etc.</param>
    /// <returns>A copy of this instance with the new output option. Other properties are preserved.</returns>
    public FFmpegArguments WithVolumeScalar(double scalar) => this with {OutputOptions = $"-af volume={scalar}"};

    /// <summary>
    /// Returns a copy of this instance with the <see cref="OutputOptions"/> set to an audio filter that modifies the volume based on relative decibels.
    /// </summary>
    /// <param name="relativeDecibels">The volume modifier in decibels. 0 = original volume.</param>
    /// <returns>A copy of this instance with the new output option. Other properties are preserved.</returns>
    public FFmpegArguments WithVolumeDecibels(double relativeDecibels) => this with {OutputOptions = $"-af volume={relativeDecibels}dB"};

    /// <summary>
    /// Sets the <see cref="InputOptions"/> to the specified string.
    /// </summary>
    /// <param name="inputOptions">The new input arguments. May be null to clear the current options.</param>
    /// <returns>A copy of this instance with the new input options. Other properties are preserved.</returns>
    public FFmpegArguments WithInputOptions(string? inputOptions) => this with {InputOptions = inputOptions};

    /// <summary>
    /// Sets the <see cref="OutputOptions"/> to the specified string.
    /// </summary>
    /// <param name="outputOptions">The new input arguments. May be null to clear the current options.</param>
    /// <returns>A copy of this instance with the new output options. Other properties are preserved.</returns>
    public FFmpegArguments WithOutputOptions(string? outputOptions) => this with {OutputOptions = outputOptions};

    /// <summary>
    /// Sets the <see cref="MuxerFormat"/> to the specified string.
    /// </summary>
    /// <param name="muxerFormat">The new muxer format. May be null to specify no format override.</param>
    /// <returns>A copy of this instance with the new muxer format. Other properties are preserved.</returns>
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

    /// <summary>
    /// Sets the <see cref="InputOptions"/> to seek to the given position in the stream.
    /// </summary>
    /// <param name="startTime">The timestamp where the stream should start.</param>
    /// <returns>A copy of this instance with the new input option. Other properties are preserved.</returns>
    public FFmpegArguments WithStreamStart(TimeSpan startTime) => this with {InputOptions = $"-ss {Format(startTime)}"};

    /// <summary>
    /// Sets the <see cref="InputOptions"/> to stop decoding after the stream has reached the given position.
    /// </summary>
    /// <param name="endTime">The timestamp where the stream should end.</param>
    /// <returns>A copy of this instance with the new input option. Other properties are preserved.</returns>
    public FFmpegArguments WithStreamEnd(TimeSpan endTime) => this with {InputOptions = $"-to {Format(endTime)}"};

    /// <summary>
    /// Sets the <see cref="InputOptions"/> to read from a range of the stream.
    /// </summary>
    /// <param name="startTime">The timestamp where the stream should start.</param>
    /// <param name="endTime">The timestamp where the stream should end.</param>
    /// <returns>A copy of this instance with the new two input options. Other properties are preserved.</returns>
    public FFmpegArguments WithStreamRange(TimeSpan startTime, TimeSpan endTime) => this with {InputOptions = $"-ss {Format(startTime)} -to {Format(endTime)}"};

}
