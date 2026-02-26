namespace SecretLabNAudio.FFmpeg.Processors;

public sealed partial class AsyncBufferedFFmpegAudioProcessor
{

    public static AsyncBufferedFFmpegAudioProcessor Create(string input, double capacitySeconds, int sampleRate, int channels)
        => new(input, capacitySeconds, WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels));

    public static AsyncBufferedFFmpegAudioProcessor Create(Func<Awaitable<Stream>> inputPipeResolver, double capacitySeconds, int sampleRate, int channels)
    {
        return new(inputPipeResolver, capacitySeconds, sampleRate, channels);
    }

}
