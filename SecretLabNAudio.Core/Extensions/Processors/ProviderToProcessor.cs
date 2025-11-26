namespace SecretLabNAudio.Core.Extensions.Processors;

public static class ProviderToProcessor
{

    public static IAudioProcessor WaveProviderToProcessor(IWaveProvider provider, bool isOwned)
        => provider is not WaveStream stream
            ? new SampleProviderWrapper(provider.ToSampleProvider())
            : new StreamAudioProcessor(stream, isOwned);

    /// <param name="provider">The sample provider to convert.</param>
    extension(ISampleProvider provider)
    {

        public IAudioProcessor ToCompatibleProcessor(bool isOwned = true)
            => (provider as IAudioProcessor ?? new SampleProviderWrapper(provider)).ToPlayerCompatible(isOwned);

        public ProcessorChain ToCompatibleChain(bool isOwned = true)
            => provider.ToCompatibleProcessor(isOwned).ToChain(isOwned);

    }

    extension(IWaveProvider provider)
    {

        public IAudioProcessor ToCompatibleProcessor(bool isOwned = true)
            => AudioProcessorExtensions.ToPlayerCompatible(WaveProviderToProcessor(provider, isOwned));

        public ProcessorChain ToCompatibleChain(bool isOwned = true) => provider.ToCompatibleProcessor(isOwned).ToChain();

    }

}
