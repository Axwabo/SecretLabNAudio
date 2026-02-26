using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Processors;

public sealed partial class SynchronousFFmpegAudioProcessor
{

    // TODO: nullable
    public static SynchronousFFmpegAudioProcessor CreatePlayerCompatible(string path) => new(
        FFmpegSL.PlayerCompatibleToStdout(path) ?? throw FFmpegStartupException.Last,
        AudioPlayer.SupportedFormat
    );

    public static SynchronousFFmpegAudioProcessor CreatePlayerCompatible(FFmpegArguments arguments) => new(
        FFmpegSL.StartRaw(arguments.ForPlayerCompatibleFloatPiping()) ?? throw FFmpegStartupException.Last,
        AudioPlayer.SupportedFormat
    );

    public static SynchronousFFmpegAudioProcessor Create(string path, int sampleRate, int channels)
        => sampleRate <= 0
            ? throw new ArgumentOutOfRangeException(nameof(sampleRate), "Sample rate must be greater than 0")
            : channels is not (1 or 2)
                ? throw new ArgumentOutOfRangeException(nameof(channels), "Channel layout must be 1 or 2")
                : new SynchronousFFmpegAudioProcessor(
                    FFmpegSL.ToStdout(path, sampleRate, channels) ?? throw FFmpegStartupException.Last,
                    WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels)
                );

    public static SynchronousFFmpegAudioProcessor Create(FFmpegArguments arguments)
    {
        if (arguments.SampleRate <= 0)
            throw new ArgumentOutOfRangeException(nameof(arguments), "Sample rate must be greater than 0");
        if (arguments.Channels is not (1 or 2))
            throw new ArgumentOutOfRangeException(nameof(arguments), "Channel layout must be 1 or 2");
        var ffmpeg = FFmpegSL.StartRaw(arguments.ForFloatPiping()) ?? throw FFmpegStartupException.Last;
        return new SynchronousFFmpegAudioProcessor(ffmpeg, WaveFormat.CreateIeeeFloatWaveFormat(arguments.SampleRate, arguments.Channels));
    }

}
