namespace SecretLabNAudio.FFmpeg.Processors;

public sealed class AsyncBufferedFFmpegAudioProcessor : AsyncFFmpegProcessorBase
{

    public static AsyncBufferedFFmpegAudioProcessor CreatePlayerCompatible(string input, double capacity = DefaultCapacity)
        => new(input, capacity, AudioPlayer.SupportedFormat);

    public static AsyncBufferedFFmpegAudioProcessor CreatePlayerCompatible(FFmpegArguments arguments, double capacity = DefaultCapacity)
        => new(capacity, arguments.ForPlayerCompatibleFloatPiping());

    public AsyncBufferedFFmpegAudioProcessor(string input, int sampleRate, int channels, double capacity = DefaultCapacity)
        : this(input, capacity, WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels))
    {
    }

    public AsyncBufferedFFmpegAudioProcessor(FFmpegArguments arguments, double capacity = DefaultCapacity)
        : this(capacity, arguments.ForFloatPiping())
    {
    }

    private AsyncBufferedFFmpegAudioProcessor(string input, double capacity, WaveFormat format) : base(capacity, format)
    {
        var arguments = FFmpegArguments.ToStdoutString(input, format.SampleRate, format.Channels);
        Run(() =>
        {
            if (TryStartFFmpeg(arguments, out var ffmpeg))
                BufferLoop(ffmpeg);
        });
    }

    private AsyncBufferedFFmpegAudioProcessor(double capacity, FFmpegArguments transformedArguments) : base(capacity, transformedArguments)
    {
        var arguments = transformedArguments.ToString();
        Run(() =>
        {
            if (TryStartFFmpeg(arguments, out var ffmpeg))
                BufferLoop(ffmpeg);
        });
    }

}
