using System.Diagnostics.CodeAnalysis;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.Extensions;

public static class AudioProcessorExtensions
{

    extension(IAudioProcessor processor)
    {

        public bool TryConvert<T>([NotNullWhen(true)] out T? result) where T : IAudioProcessor
        {
            switch (processor)
            {
                case T t:
                    result = t;
                    return true;
                case ProcessorChain {Root: T t}:
                    result = t;
                    return true;
                case ProcessorChain {Root: IAudioProcessor root}:
                    return root.TryConvert(out result);
                default:
                    result = default;
                    return false;
            }
        }

        public bool TryGetStream([NotNullWhen(true)] out WaveStream? stream)
        {
            if (processor.TryConvert(out StreamAudioProcessor? streamAudioProcessor))
            {
                stream = streamAudioProcessor.Stream;
                return true;
            }

            stream = null;
            return false;
        }

        public IAudioProcessor ToPlayerCompatible(bool isOwned = true)
            => processor.WaveFormat.SampleRate == AudioPlayer.SampleRate && processor.WaveFormat.Channels == AudioPlayer.Channels
                ? processor
                : processor.ToChain().ToPlayerCompatible();

        public ProcessorChain ToChain(bool isOwned = true) => processor as ProcessorChain ?? new ProcessorChain(processor, isOwned);

    }

}
