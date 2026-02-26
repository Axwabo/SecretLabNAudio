namespace SecretLabNAudio.FFmpeg.Processors;

public sealed partial class AsyncBufferedFFmpegAudioProcessor : AsyncFFmpegProcessorBase
{

    private AsyncBufferedFFmpegAudioProcessor(string input, double capacity, WaveFormat format) : base(capacity, format)
        => Run(() =>
        {
            if (TryStartFFmpeg(input, out var ffmpeg))
                BufferLoop(ffmpeg);
        });

}
