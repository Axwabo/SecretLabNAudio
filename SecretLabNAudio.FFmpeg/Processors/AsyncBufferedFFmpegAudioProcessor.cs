using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Processors;

/// <summary>
/// A simple, asynchronously buffered FFmpeg-based audio processor.
/// </summary>
public sealed class AsyncBufferedFFmpegAudioProcessor : AsyncFFmpegProcessorBase
{

    /// <summary>
    /// Creates a <see cref="AudioConstants.SupportedFormat">player-compatible</see> <see cref="AsyncBufferedFFmpegAudioProcessor"/> with the given input.
    /// </summary>
    /// <param name="input">The input source (e.g. file path, URL).</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <returns>A new <see cref="AsyncBufferedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Args.xml' path='doc/InArg/exception'/>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static AsyncBufferedFFmpegAudioProcessor CreatePlayerCompatible(string input, double capacity = DefaultCapacity)
        => new(input, capacity, AudioConstants.SupportedFormat);

    /// <summary>
    /// Creates a <see cref="AudioConstants.SupportedFormat">player-compatible</see> <see cref="AsyncBufferedFFmpegAudioProcessor"/> with the given arguments.
    /// </summary>
    /// <param name="arguments">The arguments to pass to FFmpeg.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <returns>A new <see cref="AsyncBufferedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Args.xml' path='doc/In/exception'/>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static AsyncBufferedFFmpegAudioProcessor CreatePlayerCompatible(FFmpegArguments arguments, double capacity = DefaultCapacity)
        => new(capacity, arguments.ForPlayerCompatibleFloatPiping());

    /// <summary>
    /// Creates an <see cref="AsyncBufferedFFmpegAudioProcessor"/> with the given input, sample rate and channel count.
    /// </summary>
    /// <param name="input">The input source (e.g. file path, URL).</param>
    /// <param name="sampleRate">The sample rate to output.</param>
    /// <param name="channels">The number of channels to output.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <returns>A new <see cref="AsyncBufferedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Args.xml' path='doc/InArg/exception'/>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static AsyncBufferedFFmpegAudioProcessor Create(string input, int sampleRate, int channels, double capacity = DefaultCapacity)
        => new(input, capacity, WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels));

    /// <summary>
    /// Creates an <see cref="AsyncBufferedFFmpegAudioProcessor"/> with the given arguments. The format will be based on the <paramref name="arguments"/>.
    /// </summary>
    /// <param name="arguments">The arguments to pass to FFmpeg.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <returns>A new <see cref="AsyncBufferedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Args.xml' path='doc/In/exception'/>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static AsyncBufferedFFmpegAudioProcessor Create(FFmpegArguments arguments, double capacity = DefaultCapacity)
        => new(capacity, arguments.ForFloatPiping());

    private AsyncBufferedFFmpegAudioProcessor(string input, double capacity, WaveFormat format) : base(capacity, format)
    {
        BufferingState = AsyncBufferingState.StartingFFmpeg;
        Start(FFmpegArguments.ToStdoutString(input, format.SampleRate, format.Channels));
    }

    private AsyncBufferedFFmpegAudioProcessor(double capacity, FFmpegArguments transformedArguments) : base(capacity, transformedArguments)
    {
        BufferingState = AsyncBufferingState.StartingFFmpeg;
        Start(transformedArguments.ToArgumentsString());
    }

    private void Start(string arguments) => Offload(() =>
    {
        if (TryStartFFmpeg(arguments, out var ffmpeg))
            BufferLoop(ffmpeg);
    });

    /// <inheritdoc />
    /// <remarks>A graceful termination signal is sent to FFmpeg. This method does not wait for FFmpeg to exit.</remarks>
    public override void StopBuffering()
    {
        base.StopBuffering();
        if (!IsDisposed && Process is {HasExited: false})
            Process.TryTerminateGracefully(0);
    }

}
