using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions.Processors;

public delegate T ProviderMapper<out T>(ISampleProvider current) where T : ISampleProvider;

public static class ProcessorChainExtensions
{

    private delegate ProcessorChain MapAndRetrieve(ProcessorChain chain, ProviderMapper mapper, bool isOwned);

    /// <param name="chain">The audio processor chain.</param>
    extension(ProcessorChain chain)
    {

        public ProcessorChain Resample(int sampleRate)
            => chain.Master.WaveFormat.SampleRate == sampleRate
                ? chain
                : chain.SwapTOrLayer<WdlResamplingSampleProvider>(provider => new WdlResamplingSampleProvider(provider, sampleRate));

        public ProcessorChain ToMono() => chain.SwapTOrLayer<MonoToStereoSampleProvider>(static provider => provider.ToMono());

        public ProcessorChain ToStereo() => chain.SwapTOrLayer<MonoToStereoSampleProvider>(static provider => provider.ToStereo());

        public ProcessorChain ToPlayerCompatible()
        {
            var format = chain.Master.WaveFormat;
            return (format.SampleRate == AudioPlayer.SampleRate, format.Channels == AudioPlayer.Channels) switch
            {
                (true, true) => chain,
                (false, false) => chain.Pop<WdlResamplingSampleProvider>().ToMono().Resample(AudioPlayer.SampleRate),
                (true, false) => chain.ToMono(),
                (false, true) => chain.Resample(AudioPlayer.SampleRate)
            };
        }

        public ProcessorChain ToFormat(int sampleRate, int channels)
        {
            var format = chain.Master.WaveFormat;
            return (format.SampleRate == sampleRate, format.Channels == channels) switch
            {
                (true, true) => chain,
                (false, false) when channels == 1 => chain.Pop<WdlResamplingSampleProvider>().ToMono().Resample(sampleRate),
                (false, false) => chain.Pop<MonoToStereoSampleProvider>().Resample(sampleRate).ToStereo(),
                (true, false) when channels == 1 => chain.ToMono(),
                (true, false) => chain.ToStereo(),
                (false, true) => chain.Resample(sampleRate)
            };
        }

        public ProcessorChain Buffer(double seconds) => chain.SwapTOrLayer<BufferedSampleProvider>(provider => new BufferedSampleProvider(provider, seconds));

        public ProcessorChain Volume(float volume = 1) => chain.SwapTOrLayer<VolumeSampleProvider>(provider => provider.Volume(volume));

        public ProcessorChain Volume(out VolumeSampleProvider provider, float volume = 1f) => chain.ConvertAndRetrieve(provider => provider.Volume(volume), out provider);

    }

    extension<T>(ProcessorChain chain)
    {

        public ProcessorChain Pop() => chain.Master is T ? chain.Pop() : chain;

        public ProcessorChain SwapTOrLayer(ProviderMapper mapper, bool isOwned = true)
            => chain.Master is T
                ? chain.Swap(mapper, isOwned)
                : chain.Layer(mapper, isOwned);

        public bool TryGetLayer([NotNullWhen(true)] out ProcessorLayer? layer, [NotNullWhen(true)] out T? provider)
        {
            foreach (var processorLayer in chain.Layers)
            {
                if (processorLayer is not {Provider: T t})
                    continue;
                layer = processorLayer;
                provider = t;
                return true;
            }

            layer = null;
            provider = default;
            return false;
        }

        public bool TryGetLayer([NotNullWhen(true)] out T? layer) => chain.TryGetLayer(out _, out layer);

    }

    extension<T>(ProcessorChain chain) where T : ISampleProvider
    {

        private static ProcessorChain LayerAndRetrieve(ProcessorChain processorChain, ProviderMapper providerMapper, bool owned)
            => processorChain.Layer(providerMapper, owned);

        public ProcessorChain Layer(ProviderMapper<T> mapper, out T provider, bool isOwned = true)
            => chain.ConvertAndRetrieve(mapper, LayerAndRetrieve<T>, out provider, isOwned);

        public ProcessorChain SwapTOrLayer(ProviderMapper<T> mapper, out T provider, bool isOwned = true)
            => chain.ConvertAndRetrieve(mapper, SwapTOrLayer<T>, out provider, isOwned);

        private ProcessorChain ConvertAndRetrieve(ProviderMapper<T> mapper, MapAndRetrieve mapAndRetrieve, out T provider, bool isOwned)
        {
            mapAndRetrieve(chain, current => mapper(current), isOwned);
            provider = (T) chain.Master;
            return chain;
        }

    }

}
