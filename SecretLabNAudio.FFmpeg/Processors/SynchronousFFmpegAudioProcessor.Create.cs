using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Processors;

public sealed partial class SynchronousFFmpegAudioProcessor
{

    public static SynchronousFFmpegAudioProcessor? CreatePlayerCompatible(string path)
        => FFmpegSL.PlayerCompatibleToStdout(path) is { } ffmpeg
            ? new SynchronousFFmpegAudioProcessor(ffmpeg, AudioPlayer.SupportedFormat)
            : null;

    public static SynchronousFFmpegAudioProcessor? CreatePlayerCompatible(FFmpegArguments arguments)
        => FFmpegSL.StartRaw(arguments.ForPlayerCompatibleFloatPiping()) is { } ffmpeg
            ? new SynchronousFFmpegAudioProcessor(ffmpeg, AudioPlayer.SupportedFormat)
            : null;

    public static SynchronousFFmpegAudioProcessor? Create(string path, int sampleRate, int channels)
        => FFmpegSL.ToStdout(path, sampleRate, channels) is { } ffmpeg
            ? new SynchronousFFmpegAudioProcessor(ffmpeg, WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels))
            : null;

    public static SynchronousFFmpegAudioProcessor? Create(FFmpegArguments arguments)
        => FFmpegSL.StartRaw(arguments.ForFloatPiping()) is { } ffmpeg
            ? new SynchronousFFmpegAudioProcessor(ffmpeg, WaveFormat.CreateIeeeFloatWaveFormat(arguments.SampleRate, arguments.Channels))
            : null;

}
