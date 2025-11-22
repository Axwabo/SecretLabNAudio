using System.Diagnostics.CodeAnalysis;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.Extensions;

public static class AudioProcessorExtensions
{

    extension(IAudioProcessor processor)
    {

        public bool TryGetStream([NotNullWhen(true)] out WaveStream? stream)
        {
            switch (processor)
            {
                case ProcessorChain {Root: IAudioProcessor root}:
                    return root.TryGetStream(out stream);
                case StreamAudioProcessor streamAudioProcessor:
                    stream = streamAudioProcessor.Stream;
                    return true;
                default:
                    stream = null;
                    return false;
            }
        }

        public IAudioProcessor ToPlayerCompatible(bool isOwned = true)
            => processor.WaveFormat.SampleRate == AudioPlayer.SampleRate && processor.WaveFormat.Channels == AudioPlayer.Channels
                ? processor
                : (processor as ProcessorChain ?? new ProcessorChain(processor, isOwned)).ToPlayerCompatible();

    }

    extension(ISampleProvider provider)
    {

        public IAudioProcessor ToPlayerCompatibleProcessor(bool isOwned = true)
            => (provider as IAudioProcessor ?? new SampleProviderWrapper(provider)).ToPlayerCompatible(isOwned);

    }

}
