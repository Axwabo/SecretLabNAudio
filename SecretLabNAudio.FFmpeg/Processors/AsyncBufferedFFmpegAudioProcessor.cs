namespace SecretLabNAudio.FFmpeg.Processors;

public sealed partial class AsyncBufferedFFmpegAudioProcessor : AsyncFFmpegProcessorBase
{

    private AsyncBufferedFFmpegAudioProcessor(string input, double capacity, WaveFormat format) : base(capacity, format)
        => Run(() =>
        {
            if (TryStartFFmpeg(FFmpegArguments.ToStdoutString(input, WaveFormat.SampleRate, WaveFormat.Channels), out var ffmpeg))
                BufferLoop(ffmpeg);
        });

    private AsyncBufferedFFmpegAudioProcessor(FFmpegArguments arguments, double capacity) : base(capacity, arguments)
        => Run(() =>
        {
            if (TryStartFFmpeg(arguments, out var ffmpeg))
                BufferLoop(ffmpeg);
        });

}
