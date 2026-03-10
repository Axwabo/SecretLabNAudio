using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Processors;

public sealed partial class SynchronousFFmpegAudioProcessor
{

    /// <summary>
    /// Creates a <see cref="AudioPlayer.SupportedFormat">player-compatible</see> <see cref="SynchronousFFmpegAudioProcessor"/>.
    /// </summary>
    /// <param name="input">The input source (e.g. file path, URL).</param>
    /// <returns>A new <see cref="SynchronousFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Args.xml' path='doc/InArg/exception'/>
    public static SynchronousFFmpegAudioProcessor? CreatePlayerCompatible(string input)
        => FFmpegSL.PlayerCompatibleToStdout(input) is { } ffmpeg
            ? new SynchronousFFmpegAudioProcessor(ffmpeg, AudioPlayer.SupportedFormat)
            : null;

    /// <summary>
    /// Creates a <see cref="AudioPlayer.SupportedFormat">player-compatible</see> <see cref="SynchronousFFmpegAudioProcessor"/>.
    /// </summary>
    /// <param name="arguments">The arguments to pass to FFmpeg.</param>
    /// <returns>A new <see cref="SynchronousFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Args.xml' path='doc/In/exception'/>
    public static SynchronousFFmpegAudioProcessor? CreatePlayerCompatible(FFmpegArguments arguments)
        => FFmpegSL.Start(arguments.ForPlayerCompatibleFloatPiping()) is { } ffmpeg
            ? new SynchronousFFmpegAudioProcessor(ffmpeg, AudioPlayer.SupportedFormat)
            : null;

    /// <summary>
    /// Creates a new <see cref="SynchronousFFmpegAudioProcessor"/>.
    /// </summary>
    /// <param name="input">The input source (e.g. file path, URL).</param>
    /// <param name="sampleRate">The sample rate to output.</param>
    /// <param name="channels">The number of channels to output.</param>
    /// <returns>A new <see cref="SynchronousFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Args.xml' path='doc/InArg/exception'/>
    public static SynchronousFFmpegAudioProcessor? Create(string input, int sampleRate, int channels)
        => FFmpegSL.ToStdout(input, sampleRate, channels) is { } ffmpeg
            ? new SynchronousFFmpegAudioProcessor(ffmpeg, WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels))
            : null;

    /// <summary>
    /// Creates a new <see cref="SynchronousFFmpegAudioProcessor"/>. The format will be based on the <paramref name="arguments"/>.
    /// </summary>
    /// <param name="arguments">The arguments to pass to FFmpeg.</param>
    /// <returns>A new <see cref="SynchronousFFmpegAudioProcessor"/>.</returns>
    /// <include file='../XmlDocs/Args.xml' path='doc/In/exception'/>
    public static SynchronousFFmpegAudioProcessor? Create(FFmpegArguments arguments)
        => FFmpegSL.Start(arguments.ForFloatPiping()) is { } ffmpeg
            ? new SynchronousFFmpegAudioProcessor(ffmpeg, arguments)
            : null;

}
