namespace SecretLabNAudio.Core.Extensions.Processors;

/// <summary>Methods to convert an NAudio <see cref="ISampleProvider"/> into an <see cref="IAudioProcessor"/>.</summary>
public static class ProviderToProcessor
{

    /// <summary>
    /// Converts the wave provider to a sample provider, and wraps it in an appropriate <see cref="IAudioProcessor"/>.
    /// </summary>
    /// <param name="provider">The provider to wrap.</param>
    /// <param name="isOwned">Whether to dispose of the provider when the processor is disposed.</param>
    /// <returns>A <see cref="StreamAudioProcessor"/> if the provider is a <see cref="WaveStream"/>, otherwise, a <see cref="SampleProviderWrapper"/>.</returns>
    public static IAudioProcessor WaveProviderToProcessor(IWaveProvider provider, bool isOwned)
        => provider is WaveStream stream
            ? new StreamAudioProcessor(stream, isOwned)
            : isOwned
                ? new SampleProviderWrapper(provider.ToSampleProvider(), provider as IDisposable)
                : new SampleProviderWrapper(provider.ToSampleProvider());

    /// <summary>
    /// Safely casts the sample provider to an <see cref="IAudioProcessor"/> or wraps it.
    /// </summary>
    /// <param name="provider">The provider to convert.</param>
    /// <returns>The provider as an <see cref="IAudioProcessor"/> or a new <see cref="SampleProviderWrapper"/> if the provider isn't an audio processor.</returns>
    public static IAudioProcessor SampleProviderToProcessor(ISampleProvider provider) => provider as IAudioProcessor ?? new SampleProviderWrapper(provider);

    /// <param name="provider">The sample provider to convert.</param>
    extension(ISampleProvider provider)
    {

        /// <summary>
        /// Wraps the provider in a <see cref="SampleProviderWrapper"/>, and ensures that its format matches <see cref="AudioPlayer.SupportedFormat"/>.
        /// </summary>
        /// <param name="isOwned">Whether to dispose of the provider if format conversion is required.</param>
        /// <returns>A player-compatible <see cref="IAudioProcessor"/> (<see cref="ProcessorChain"/> if conversion was performed).</returns>
        public IAudioProcessor ToCompatibleProcessor(bool isOwned = true)
            => SampleProviderToProcessor(provider).ToPlayerCompatible(isOwned);

        // TODO: lazy docs??
        /// <summary>
        /// Creates a <see cref="ProcessorChain"/>
        /// </summary>
        /// <param name="isOwned"></param>
        /// <returns></returns>
        public ProcessorChain ToCompatibleChain(bool isOwned = true)
            => provider.ToCompatibleProcessor(isOwned).ToChain(isOwned);

        internal ISampleProvider Process(ModifyChain? process)
        {
            if (process == null)
                return provider;
            var chain = SampleProviderToProcessor(provider).ToCompatibleChain();
            process(chain);
            return chain;
        }

    }

    /// <param name="provider">The provider to wrap.</param>
    extension(IWaveProvider provider)
    {

        public IAudioProcessor ToCompatibleProcessor(bool isOwned = true)
            => WaveProviderToProcessor(provider, isOwned).ToPlayerCompatible();

        public ProcessorChain ToCompatibleChain(bool isOwned = true) => provider.ToCompatibleProcessor(isOwned).ToChain();

    }

}
