namespace SecretLabNAudio.Core.Extensions.Processors;

public static class ProviderToProcessor
{

    public static IAudioProcessor WaveProviderToProcessor(IWaveProvider provider, bool isOwned)
        => provider is not WaveStream stream
            ? new SampleProviderWrapper(provider.ToSampleProvider())
            : new StreamAudioProcessor(stream, isOwned);

    public static IAudioProcessor SampleProviderToProcessor(ISampleProvider provider) => (provider as IAudioProcessor ?? new SampleProviderWrapper(provider));

    /// <param name="provider">The sample provider to convert.</param>
    extension(ISampleProvider provider)
    {

        public IAudioProcessor ToCompatibleProcessor(bool isOwned = true)
            => SampleProviderToProcessor(provider).ToPlayerCompatible(isOwned);

        public ProcessorChain ToCompatibleChain(bool isOwned = true)
            => provider.ToCompatibleProcessor(isOwned).ToChain(isOwned);

        internal ISampleProvider Process(Process? process)
        {
            if (process == null)
                return provider;
            var chain = SampleProviderToProcessor(provider).ToCompatibleChain();
            process(chain);
            return chain;
        }

    }

    extension(IWaveProvider provider)
    {

        public IAudioProcessor ToCompatibleProcessor(bool isOwned = true)
            => WaveProviderToProcessor(provider, isOwned).ToPlayerCompatible();

        public ProcessorChain ToCompatibleChain(bool isOwned = true) => provider.ToCompatibleProcessor(isOwned).ToChain();

    }

}
