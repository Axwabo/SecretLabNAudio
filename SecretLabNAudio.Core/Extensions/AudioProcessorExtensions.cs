using System.Diagnostics.CodeAnalysis;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.Extensions;

public static class AudioProcessorExtensions
{

    extension(IAudioProcessor processor)
    {

        public bool TryGetRootAs<T>([NotNullWhen(true)] out T? result)
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
                    return root.TryGetRootAs(out result);
                default:
                    result = default;
                    return false;
            }
        }

        public bool TryGetMasterAs<T>([NotNullWhen(true)] out T? result)
        {
            switch (processor)
            {
                case T t:
                    result = t;
                    return true;
                case ProcessorChain {Master: T t}:
                    result = t;
                    return true;
                case ProcessorChain {Master: IAudioProcessor root}:
                    return root.TryGetRootAs(out result);
                default:
                    result = default;
                    return false;
            }
        }

        public IAudioProcessor ToPlayerCompatible(bool isOwned = true)
            => processor.WaveFormat.SampleRate == AudioPlayer.SampleRate && processor.WaveFormat.Channels == AudioPlayer.Channels
                ? processor
                : processor.ToChain(isOwned).ToPlayerCompatible(isOwned);

        public ProcessorChain ToChain(bool isOwned = true) => processor as ProcessorChain ?? new ProcessorChain(processor, isOwned);

    }

}
