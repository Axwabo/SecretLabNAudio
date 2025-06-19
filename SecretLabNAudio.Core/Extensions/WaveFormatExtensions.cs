namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="WaveFormat"/> class.</summary>
public static class WaveFormatExtensions
{

    /// <summary>
    /// Calculates the amount of samples needed for the specified amount of seconds based on the <see cref="WaveFormat"/>. 
    /// </summary>
    /// <param name="format">The <see cref="WaveFormat"/> to calculate the sample count for.</param>
    /// <param name="seconds">The amount of seconds to calculate the sample count for.</param>
    /// <returns>The number of samples in the specified amount of seconds.</returns>
    public static int SampleCount(this WaveFormat format, double seconds)
        => SampleCount(seconds, format.SampleRate, format.Channels);

    /// <summary>
    /// Calculates the amount of samples needed for the specified amount of seconds based on the sample rate and number of channels.
    /// </summary>
    /// <param name="seconds">The amount of seconds to calculate the sample count for.</param>
    /// <param name="sampleRate">The sample rate to use for the calculation.</param>
    /// <param name="channels">The number of channels to use for the calculation.</param>
    /// <returns>The number of samples in the specified amount of seconds.</returns>
    public static int SampleCount(double seconds, int sampleRate, int channels) => (int) (seconds * sampleRate * channels);

    /// <summary>
    /// Calculates the duration in seconds for the specified number of samples based on the <see cref="WaveFormat"/>.
    /// </summary>
    /// <param name="format">The <see cref="WaveFormat"/> to calculate the duration for.</param>
    /// <param name="samples">The number of samples to calculate the duration for.</param>
    /// <returns>The duration in seconds for the specified number of samples.</returns>
    public static double Seconds(this WaveFormat format, int samples)
        => Seconds(samples, format.SampleRate, format.Channels);

    /// <summary>
    /// Calculates the duration in seconds for the specified number of samples based on the sample rate and number of channels.
    /// </summary>
    /// <param name="samples">The number of samples to calculate the duration for.</param>
    /// <param name="sampleRate">The sample rate to use for the calculation.</param>
    /// <param name="channels">The number of channels to use for the calculation.</param>
    /// <returns>The duration in seconds for the specified number of samples.</returns>
    public static double Seconds(int samples, int sampleRate, int channels)
        => (double) samples / (sampleRate * channels);

}
