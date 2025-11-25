using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.Extensions.Providers;

public static class ProviderToProcessor
{

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
            => provider is not WaveStream stream
                ? AudioProcessorExtensions.ToPlayerCompatible(new SampleProviderWrapper(provider.ToSampleProvider()))
                : AudioProcessorExtensions.ToPlayerCompatible(new StreamAudioProcessor(stream, isOwned));

        public ProcessorChain ToCompatibleChain(bool isOwned = true) => provider.ToCompatibleProcessor(isOwned).ToChain();

    }

}
