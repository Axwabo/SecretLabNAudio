using System.Threading.Tasks;

namespace SecretLabNAudio.FFmpeg.Processors;

using StreamResolver = Func<CancellationToken, Task<Stream>>;

/// <summary>
/// An asynchronously buffered FFmpeg-based audio processor that pipes a <see cref="Stream"/> to the FFmpeg process.
/// The stream is resolved on a background thread.
/// </summary>
public sealed partial class StreamBasedFFmpegAudioProcessor
{

    private static readonly FFmpegArguments PlayerCompatibleArguments = FFmpegArguments.PlayerCompatibleStdout with {Input = FFmpegArguments.StandardPipe};

    /// <summary>
    /// Creates a <see cref="AudioConstants.SupportedFormat">player-compatible</see> <see cref="StreamBasedFFmpegAudioProcessor"/> with the given stream resolver delegate.
    /// </summary>
    /// <param name="resolver">The delegate to invoke on a background thread which should return a <see cref="Task"/> of <see cref="Stream"/>.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="isOwned">Whether to dispose of the resolved stream after copying fails, finishes or gets canceled.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(StreamResolver resolver, double capacity = DefaultCapacity, bool isOwned = true)
        => new(resolver, isOwned, capacity, PlayerCompatibleArguments);

    /// <summary>
    /// Creates a <see cref="AudioConstants.SupportedFormat">player-compatible</see> <see cref="StreamBasedFFmpegAudioProcessor"/> with the given stream resolver task.
    /// </summary>
    /// <param name="resolver">A task representing the asynchronous stream resolver.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="isOwned">Whether to dispose of the resolved stream after copying fails, finishes or gets canceled.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(Task<Stream> resolver, double capacity = DefaultCapacity, bool isOwned = true)
        => CreatePlayerCompatible(_ => resolver, capacity, isOwned);

    /// <summary>
    /// Creates a <see cref="AudioConstants.SupportedFormat">player-compatible</see> <see cref="StreamBasedFFmpegAudioProcessor"/> with the given stream.
    /// </summary>
    /// <param name="stream">The stream to pipe to FFmpeg.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="isOwned">Whether to dispose of the resolved stream after copying fails, finishes or gets canceled.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(Stream stream, double capacity = DefaultCapacity, bool isOwned = true)
        => CreatePlayerCompatible(Task.FromResult(stream), capacity, isOwned);

    /// <summary>
    /// Creates a <see cref="AudioConstants.SupportedFormat">player-compatible</see> <see cref="StreamBasedFFmpegAudioProcessor"/> with the given stream resolver delegate.
    /// </summary>
    /// <param name="resolver">The delegate to invoke on a background thread which should return a <see cref="Task"/> of <see cref="Stream"/>.</param>
    /// <param name="arguments">The arguments describing input and output options.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="isOwned">Whether to dispose of the resolved stream after copying fails, finishes or gets canceled.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <remarks>
    /// Only the <see cref="FFmpegArguments.InputOptions"/> and <see cref="FFmpegArguments.OutputOptions"/> are retained.
    /// By default, the <see cref="AsyncFFmpegProcessorBase.SleepThresholdSeconds" /> will be set to 75% of the <paramref name="capacity" />.
    /// </remarks>
    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(StreamResolver resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => new(resolver, isOwned, capacity, PlayerCompatibleArguments with {InputOptions = arguments.InputOptions, OutputOptions = arguments.OutputOptions});

    /// <summary>
    /// Creates a <see cref="AudioConstants.SupportedFormat">player-compatible</see> <see cref="StreamBasedFFmpegAudioProcessor"/> with the given stream resolver delegate.
    /// </summary>
    /// <param name="resolver">The delegate to invoke on a background thread which should return a <see cref="Task"/> of <see cref="Stream"/>.</param>
    /// <param name="arguments">The arguments describing input and output options.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="isOwned">Whether to dispose of the resolved stream after copying fails, finishes or gets canceled.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <remarks>
    /// Only the <see cref="FFmpegArguments.InputOptions"/> and <see cref="FFmpegArguments.OutputOptions"/> are retained.
    /// By default, the <see cref="AsyncFFmpegProcessorBase.SleepThresholdSeconds" /> will be set to 75% of the <paramref name="capacity" />.
    /// </remarks>
    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(Task<Stream> resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => CreatePlayerCompatible(_ => resolver, arguments, capacity, isOwned);

    /// <summary>
    /// Creates a <see cref="AudioConstants.SupportedFormat">player-compatible</see> <see cref="StreamBasedFFmpegAudioProcessor"/> with the given stream resolver delegate.
    /// </summary>
    /// <param name="stream">The stream to pipe to FFmpeg.</param>
    /// <param name="arguments">The arguments describing input and output options.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="isOwned">Whether to dispose of the resolved stream after copying fails, finishes or gets canceled.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <remarks>
    /// Only the <see cref="FFmpegArguments.InputOptions"/> and <see cref="FFmpegArguments.OutputOptions"/> are retained.
    /// By default, the <see cref="AsyncFFmpegProcessorBase.SleepThresholdSeconds" /> will be set to 75% of the <paramref name="capacity" />.
    /// </remarks>
    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(Stream stream, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => CreatePlayerCompatible(Task.FromResult(stream), arguments, capacity, isOwned);

    /// <summary>
    /// Creates a <see cref="StreamBasedFFmpegAudioProcessor"/> with the given stream resolver delegate.
    /// </summary>
    /// <param name="resolver">The delegate to invoke on a background thread which should return a <see cref="Task"/> of <see cref="Stream"/>.</param>
    /// <param name="sampleRate">The sample rate to output.</param>
    /// <param name="channels">The number of channels to output.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="isOwned">Whether to dispose of the resolved stream after copying fails, finishes or gets canceled.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static StreamBasedFFmpegAudioProcessor Create(StreamResolver resolver, int sampleRate, int channels, double capacity = DefaultCapacity, bool isOwned = true)
        => new(resolver, isOwned, capacity, FFmpegArguments.StdinToStdout(sampleRate, channels));

    /// <summary>
    /// Creates a <see cref="StreamBasedFFmpegAudioProcessor"/> with the given stream resolver task.
    /// </summary>
    /// <param name="resolver">A task representing the asynchronous stream resolver.</param>
    /// <param name="sampleRate">The sample rate to output.</param>
    /// <param name="channels">The number of channels to output.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="isOwned">Whether to dispose of the resolved stream after copying fails, finishes or gets canceled.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static StreamBasedFFmpegAudioProcessor Create(Task<Stream> resolver, int sampleRate, int channels, double capacity = DefaultCapacity, bool isOwned = true)
        => Create(_ => resolver, sampleRate, channels, capacity, isOwned);

    /// <summary>
    /// Creates a <see cref="StreamBasedFFmpegAudioProcessor"/> with the given stream.
    /// </summary>
    /// <param name="stream">The stream to pipe to FFmpeg.</param>
    /// <param name="sampleRate">The sample rate to output.</param>
    /// <param name="channels">The number of channels to output.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="isOwned">Whether to dispose of the resolved stream after copying fails, finishes or gets canceled.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static StreamBasedFFmpegAudioProcessor Create(Stream stream, int sampleRate, int channels, double capacity = DefaultCapacity, bool isOwned = true)
        => Create(Task.FromResult(stream), sampleRate, channels, capacity, isOwned);

    /// <summary>
    /// Creates a <see cref="StreamBasedFFmpegAudioProcessor"/> with the given stream resolver delegate. The format will be based on the <paramref name="arguments"/>.
    /// </summary>
    /// <param name="resolver">The delegate to invoke on a background thread which should return a <see cref="Task"/> of <see cref="Stream"/>.</param>
    /// <param name="arguments">The arguments to pass to FFmpeg.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="isOwned">Whether to dispose of the resolved stream after copying fails, finishes or gets canceled.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static StreamBasedFFmpegAudioProcessor Create(StreamResolver resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => new(resolver, isOwned, capacity, arguments.ReadFromStandardInput().ForFloatPiping());

    /// <summary>
    /// Creates a <see cref="StreamBasedFFmpegAudioProcessor"/> with the given stream resolver task. The format will be based on the <paramref name="arguments"/>.
    /// </summary>
    /// <param name="resolver">A task representing the asynchronous stream resolver.</param>
    /// <param name="arguments">The arguments to pass to FFmpeg.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="isOwned">Whether to dispose of the resolved stream after copying fails, finishes or gets canceled.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static StreamBasedFFmpegAudioProcessor Create(Task<Stream> resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => Create(_ => resolver, arguments, capacity, isOwned);

    /// <summary>
    /// Creates a <see cref="StreamBasedFFmpegAudioProcessor"/> with the given stream. The format will be based on the <paramref name="arguments"/>.
    /// </summary>
    /// <param name="stream">The stream to pipe to FFmpeg.</param>
    /// <param name="arguments">The arguments to pass to FFmpeg.</param>
    /// <param name="capacity">The capacity of the buffer in seconds.</param>
    /// <param name="isOwned">Whether to dispose of the resolved stream after copying fails, finishes or gets canceled.</param>
    /// <returns>A new <see cref="StreamBasedFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
    public static StreamBasedFFmpegAudioProcessor Create(Stream stream, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => Create(Task.FromResult(stream), arguments, capacity, isOwned);

}
