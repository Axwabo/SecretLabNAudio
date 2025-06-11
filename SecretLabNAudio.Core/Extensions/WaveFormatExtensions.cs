namespace SecretLabNAudio.Core.Extensions;

public static class WaveFormatExtensions
{

    public static int SampleCount(this WaveFormat format, double seconds)
        => SampleCount(seconds, format.SampleRate, format.Channels);

    public static int SampleCount(double seconds, int sampleRate, int channels) => (int) (seconds * sampleRate * channels);

    public static double Seconds(this WaveFormat format, int samples)
        => Seconds(samples, format.SampleRate, format.Channels);

    public static double Seconds(int samples, int sampleRate, int channels)
        => (double) samples / (sampleRate * channels);

}
