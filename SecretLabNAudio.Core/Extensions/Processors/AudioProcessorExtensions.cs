using System.Diagnostics.CodeAnalysis;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.Extensions.Processors;

public static class AudioProcessorExtensions
{

    extension(IAudioProcessor processor)
    {

        public bool TryGetSourceAs<T>([NotNullWhen(true)] out T? result)
        {
            switch (processor)
            {
                case T t:
                    result = t;
                    return true;
                case ProcessorChain {Source: T t}:
                    result = t;
                    return true;
                case ProcessorChain {Source: IAudioProcessor source}:
                    return source.TryGetSourceAs(out result);
                case Mixer mixer:
                    return mixer.TryGetSingleMixerInput(out result);
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
                case ProcessorChain {Master: IAudioProcessor master}:
                    return master.TryGetMasterAs(out result);
                case Mixer mixer:
                    return mixer.TryGetSingleMixerInput(out result);
                default:
                    result = default;
                    return false;
            }
        }

        public bool TryGetSingleMixerInput<T>([NotNullWhen(true)] out T? result)
        {
            switch (processor)
            {
                case Mixer {Inputs: [{Provider: T t}]}:
                    result = t;
                    return true;
                case ProcessorChain {Master: IAudioProcessor master}:
                    return master.TryGetSingleMixerInput(out result);
                default:
                    result = default;
                    return false;
            }
        }

        public IAudioProcessor ToPlayerCompatible(bool isOwned = true)
            => processor.WaveFormat.Matches(AudioPlayer.SampleRate, AudioPlayer.Channels)
                ? processor
                : processor.ToChain(isOwned).ToPlayerCompatible();

        public IAudioProcessor ToFormat(int sampleRate, int channels, bool isOwned = true)
            => processor.WaveFormat.Matches(sampleRate, channels)
                ? processor
                : processor.ToChain(isOwned).ToFormat(sampleRate, channels);

        public ProcessorChain ToChain(bool isOwned = true) => processor as ProcessorChain ?? new ProcessorChain(processor, isOwned);

    }

}
