using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Processors;

public sealed class AsyncBufferedFFmpegAudioProcessor : AsyncFFmpegProcessorBase
{

    public static AsyncBufferedFFmpegAudioProcessor CreatePlayerCompatible(string input, double capacity = DefaultCapacity)
        => new(input, capacity, AudioPlayer.SupportedFormat);

    public static AsyncBufferedFFmpegAudioProcessor CreatePlayerCompatible(FFmpegArguments arguments, double capacity = DefaultCapacity)
        => new(capacity, arguments.ForPlayerCompatibleFloatPiping());

    public static AsyncBufferedFFmpegAudioProcessor Create(string input, int sampleRate, int channels, double capacity = DefaultCapacity)
        => new(input, capacity, WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels));

    public static AsyncBufferedFFmpegAudioProcessor Create(FFmpegArguments arguments, double capacity = DefaultCapacity)
        => new(capacity, arguments.ForFloatPiping());

    private AsyncBufferedFFmpegAudioProcessor(string input, double capacity, WaveFormat format) : base(capacity, format)
    {
        var arguments = FFmpegArguments.ToStdoutString(input, format.SampleRate, format.Channels);
        Offload(() =>
        {
            if (TryStartFFmpeg(arguments, out var ffmpeg))
                BufferLoop(ffmpeg);
        });
    }

    private AsyncBufferedFFmpegAudioProcessor(double capacity, FFmpegArguments transformedArguments) : base(capacity, transformedArguments)
    {
        var arguments = transformedArguments.ToString();
        Offload(() =>
        {
            if (TryStartFFmpeg(arguments, out var ffmpeg))
                BufferLoop(ffmpeg);
        });
    }

    /// <inheritdoc />
    /// <remarks>A graceful termination signal is sent to FFmpeg. This method does not wait for FFmpeg to exit.</remarks>
    public override void StopBuffering()
    {
        base.StopBuffering();
        if (!IsDisposed && Process is {HasExited: false})
            Process.TryTerminateGracefully(0);
    }

}
