using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Processors;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions.Processors;

public static class ProcessorChainExtensions
{

    /// <param name="chain">The audio processor chain.</param>
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

        public ProcessorChain Volume(float volume) => chain.SwapTOrLayer<VolumeSampleProvider>(provider => provider.Volume(volume));

    }

}
