using NAudio.Wave;

namespace SecretLabNAudio.Core.Extensions;

public static class WaveFormatExtensions
{

    public static int SampleCount(this WaveFormat format, double seconds)
        => SampleCount(seconds, format.SampleRate, format.Channels);

    public static int SampleCount(double seconds, int sampleRate, int channels) => (int) (seconds * sampleRate * channels);

}
