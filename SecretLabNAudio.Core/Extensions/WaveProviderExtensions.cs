namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="IWaveProvider"/> interface.</summary>
public static class WaveProviderExtensions
{

    /// <summary>
    /// Converts the wave provider to an <see cref="AudioPlayer"/>-compatible <see cref="ISampleProvider"/>.
    /// </summary>
    /// <param name="waveProvider">Wave provider to convert.</param>
    /// <returns>An <see cref="ISampleProvider"/> that is compatible with the <see cref="AudioPlayer"/>.</returns>
    /// <seealso cref="SampleProviderExtensions.ToPlayerCompatible"/>
    public static ISampleProvider ToPlayerCompatible(this IWaveProvider waveProvider)
        => waveProvider.ToSampleProvider().ToPlayerCompatible();

}
