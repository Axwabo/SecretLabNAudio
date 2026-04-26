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
            : new SampleProviderWrapper(provider.ToSampleProvider(), isOwned ? provider as IDisposable : null);

    /// <summary>
    /// Safely casts the sample provider to an <see cref="IAudioProcessor"/>, or wraps it in a <see cref="SampleProviderWrapper"/>.
    /// </summary>
    /// <param name="provider">The provider to convert.</param>
    /// <param name="isOwned">Whether to depose of the provider when the new processor is disposed. Does nothing if the provider doesn't implement <see cref="IDisposable"/>.</param>
    /// <returns>The provider as an <see cref="IAudioProcessor"/> or a new <see cref="SampleProviderWrapper"/> if the provider isn't an audio processor.</returns>
    public static IAudioProcessor SampleProviderToProcessor(ISampleProvider provider, bool isOwned)
        => provider as IAudioProcessor ?? new SampleProviderWrapper(provider, isOwned ? provider as IDisposable : null);

    /// <param name="provider">The sample provider to convert.</param>
    extension(ISampleProvider provider)
    {

        /// <summary>
        /// Wraps the provider in a <see cref="SampleProviderWrapper"/>, and ensures that its format matches <see cref="AudioConstants.SupportedFormat"/>.
        /// </summary>
        /// <param name="isOwned">Whether to dispose of the provider if format conversion is required. Does nothing if the provider doesn't implement <see cref="IDisposable"/>.</param>
        /// <returns>A player-compatible <see cref="IAudioProcessor"/> (<see cref="ProcessorChain"/> if conversion was performed).</returns>
        public IAudioProcessor ToCompatibleProcessor(bool isOwned = true)
            => SampleProviderToProcessor(provider, isOwned).ToPlayerCompatible(isOwned);

        /// <summary>
        /// Creates a <see cref="ProcessorChain"/> from the provider, and ensures that its format matches <see cref="AudioConstants.SupportedFormat"/>.
        /// </summary>
        /// <param name="isOwned">Whether to dispose of the provider when the chain is disposed. Does nothing if the provider doesn't implement <see cref="IDisposable"/>.</param>
        /// <returns>A player-compatible <see cref="ProcessorChain"/>.</returns>
        public ProcessorChain ToCompatibleChain(bool isOwned = true)
            => provider.ToCompatibleProcessor(isOwned).ToChain(isOwned);

        internal ISampleProvider Process(ModifyChain? process)
        {
            if (process == null)
                return provider;
            var chain = SampleProviderToProcessor(provider, false).ToCompatibleChain(false);
            process(chain);
            return chain;
        }

    }

    /// <param name="provider">The provider to wrap.</param>
    extension(IWaveProvider provider)
    {

        /// <summary>
        /// Converts the wave provider to an <see cref="IAudioProcessor"/>, and ensures that its format matches <see cref="AudioConstants.SupportedFormat"/>.
        /// </summary>
        /// <param name="isOwned">Whether to dispose of the provider when the processor is disposed.</param>
        /// <returns>A player-compatible <see cref="ProcessorChain"/>, <see cref="StreamAudioProcessor"/> or <see cref="SampleProviderWrapper"/>.</returns>
        /// <seealso cref="WaveProviderToProcessor"/>
        public IAudioProcessor ToCompatibleProcessor(bool isOwned = true)
            => WaveProviderToProcessor(provider, isOwned).ToPlayerCompatible();

        /// <summary>
        /// Creates a <see cref="ProcessorChain"/> from the wave provider, and ensures that its format matches <see cref="AudioConstants.SupportedFormat"/>.
        /// </summary>
        /// <param name="isOwned">Whether to dispose of the provider when the chain is disposed. Does nothing if the provider doesn't implement <see cref="IDisposable"/>.</param>
        /// <returns>A player-compatible <see cref="ProcessorChain"/>.</returns>
        public ProcessorChain ToCompatibleChain(bool isOwned = true) => provider.ToCompatibleProcessor(isOwned).ToChain(isOwned);

    }

}
