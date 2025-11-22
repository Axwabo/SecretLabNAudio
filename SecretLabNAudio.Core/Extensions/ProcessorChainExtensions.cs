using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Processors;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions;

public static class ProcessorChainExtensions
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chain"></param>
    extension(ProcessorChain chain)
    {

        public ProcessorChain Pop<T>() => chain.Master is T ? chain.Pop() : chain;

        public ProcessorChain SwapTOrLayer<T>(ProviderMapper mapper, bool isOwned = true)
            => chain.Master is T
                ? chain.Swap(mapper, isOwned)
                : chain.Layer(mapper, isOwned);

        public ProcessorChain Resample(int sampleRate)
            => chain.Master.WaveFormat.SampleRate == sampleRate
                ? chain
                : chain.SwapTOrLayer<WdlResamplingSampleProvider>(provider => new WdlResamplingSampleProvider(provider, sampleRate));

        public ProcessorChain ToMono() => chain.SwapTOrLayer<MonoToStereoSampleProvider>(static provider => provider.ToMono());

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

        public ProcessorChain Buffer(double seconds) => chain.SwapTOrLayer<BufferedSampleProvider>(provider => new BufferedSampleProvider(provider, seconds));

        public ProcessorChain Volume(float volume)
        {
            if (chain.Master is VolumeSampleProvider volumeSampleProvider)
                volumeSampleProvider.Volume = volume;
            else
                chain.Layer(provider => provider.Volume(volume));
            return chain;
        }

    }

}
