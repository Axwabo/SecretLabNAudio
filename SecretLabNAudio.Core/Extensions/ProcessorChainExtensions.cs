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

        public ProcessorChain Pop<T>() => chain.Last is T ? chain.Pop() : chain;

        public ProcessorChain Resample(int sampleRate)
            => chain.Pop<WdlResamplingSampleProvider>()
                .Layer(provider => new WdlResamplingSampleProvider(provider, sampleRate));

    }

}
