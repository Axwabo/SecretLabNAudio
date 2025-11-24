using SecretLabNAudio.Core.Extensions.Providers;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="IWaveProvider"/> interface.</summary>
public static class WaveProviderExtensions
{

    /// <param name="waveProvider">Wave provider to convert.</param>
    extension(IWaveProvider waveProvider)
    {

        /// <inheritdoc cref="NonProcessorExtensions.ToPlayerCompatible(IWaveProvider)"/>
        [Obsolete($"Use Providers.{nameof(NonProcessorExtensions)}.{nameof(NonProcessorExtensions.ToPlayerCompatible)} instead.", true)]
        public ISampleProvider ToPlayerCompatible() => NonProcessorExtensions.ToPlayerCompatible(waveProvider);

    }

}
