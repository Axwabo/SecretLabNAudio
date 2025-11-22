using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Processors;

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
            switch (format.SampleRate == AudioPlayer.SampleRate, format.Channels == AudioPlayer.Channels)
            {
                case (true, true):
                    return chain;
                case (false, false):
                    return chain.Pop<WdlResamplingSampleProvider>();
                case (true, false):
                    return chain.ToMono();
                case (false, true):
                    return chain.Resample(AudioPlayer.SampleRate);
            }
        }

    }

}
