namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="WaveFormat"/> class.</summary>
public static class WaveFormatExtensions
{

    /// <param name="format">The <see cref="WaveFormat"/> reference.</param>
    extension(WaveFormat format)
    {

        /// <summary>
        /// Calculates the amount of samples needed for the specified amount of seconds based on the <see cref="WaveFormat"/>. 
        /// </summary>
        /// <param name="seconds">The amount of seconds to calculate the sample count for.</param>
        /// <returns>The number of samples in the specified amount of seconds.</returns>
        public int SampleCount(double seconds) => SampleCount(seconds, format.SampleRate, format.Channels);

        /// <summary>
        /// Calculates the duration in seconds for the specified number of samples based on the <see cref="WaveFormat"/>.
        /// </summary>
        /// <param name="samples">The number of samples to calculate the duration for.</param>
        /// <returns>The duration in seconds for the specified number of samples.</returns>
        public double Seconds(int samples) => Seconds(samples, format.SampleRate, format.Channels);

        /// <summary>
        /// Calculates the duration as a <see cref="TimeSpan"/> for the specified number of samples based on the <see cref="WaveFormat"/>.
        /// </summary>
        /// <param name="samples">The number of samples to calculate the duration for.</param>
        /// <returns>The duration in seconds for the specified number of samples.</returns>
        public TimeSpan Time(int samples) => TimeSpan.FromSeconds(format.Seconds(samples));

        /// <summary>
        /// Checks whether the format matches a certain sample rate and channel count.
        /// </summary>
        /// <param name="sampleRate">The sample rate to match.</param>
        /// <param name="channels">The number of channels to match.</param>
        /// <returns>Whether both properties match.</returns>
        public bool Matches(int sampleRate, int channels) => format.SampleRate == sampleRate && format.Channels == channels;

        /// <summary>
        /// Checks whether the format matches the <paramref name="other"/>'s sample rate and channel count.
        /// </summary>
        /// <param name="other">The format to match.</param>
        /// <returns>Whether the sample rate and channel count match.</returns>
        public bool Matches(WaveFormat other) => format.Matches(other.SampleRate, other.Channels);

    }

    /// <summary>
    /// Calculates the amount of samples needed for the specified amount of seconds based on the sample rate and number of channels.
    /// </summary>
    /// <param name="seconds">The amount of seconds to calculate the sample count for.</param>
    /// <param name="sampleRate">The sample rate to use for the calculation.</param>
    /// <param name="channels">The number of channels to use for the calculation.</param>
    /// <returns>The number of samples in the specified amount of seconds.</returns>
    public static int SampleCount(double seconds, int sampleRate, int channels) => (int) (seconds * sampleRate * channels);

    /// <summary>
    /// Calculates the duration in seconds for the specified number of samples based on the sample rate and number of channels.
    /// </summary>
    /// <param name="samples">The number of samples to calculate the duration for.</param>
    /// <param name="sampleRate">The sample rate to use for the calculation.</param>
    /// <param name="channels">The number of channels to use for the calculation.</param>
    /// <returns>The duration in seconds for the specified number of samples.</returns>
    public static double Seconds(int samples, int sampleRate, int channels) => (double) samples / (sampleRate * channels);

}
