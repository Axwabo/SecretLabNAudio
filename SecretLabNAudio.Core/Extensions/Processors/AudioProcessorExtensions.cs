namespace SecretLabNAudio.Core.Extensions.Processors;

/// <summary>Extension methods for the <see cref="IAudioProcessor"/> interface.</summary>
public static class AudioProcessorExtensions
{

    /// <param name="processor">The audio processor to modify.</param>
    extension(IAudioProcessor processor)
    {

        /// <summary>
        /// Mixes down and resamples the processor to be compatible with <see cref="AudioPlayer.SupportedFormat"/> if needed.
        /// </summary>
        /// <param name="isOwned">Whether to dispose this processor when the newly created <see cref="ProcessorChain"/> is disposed.</param>
        /// <returns>The processor itself if the format is already compatible, otherwise, a player-compatible <see cref="ProcessorChain"/>.</returns>
        public IAudioProcessor ToPlayerCompatible(bool isOwned = true)
            => processor.WaveFormat.Matches(AudioPlayer.SampleRate, AudioPlayer.Channels)
                ? processor
                : processor.ToChain(isOwned).ToPlayerCompatible();

        /// <summary>
        /// Resamples and changes the channel layout to be compatible with the given sample rate and channel count if needed.
        /// </summary>
        /// <inheritdoc cref="ProcessorChainExtensions.ToFormat"/>
        /// <returns>The processor itself if the format is already compatible, otherwise, a compatible <see cref="ProcessorChain"/>.</returns>
        public IAudioProcessor ToFormat(int sampleRate, int channels, bool isOwned = true)
            => processor.WaveFormat.Matches(sampleRate, channels)
                ? processor
                : processor.ToChain(isOwned).ToFormat(sampleRate, channels);

        /// <summary>
        /// Safely casts the processor to a <see cref="ProcessorChain"/> or wraps it in one.
        /// </summary>
        /// <param name="isOwned">Whether to dispose of this processor when the newly created <see cref="ProcessorChain"/> is disposed.</param>
        /// <returns>The processor as a chain or a newly created chain with the processor as the source.</returns>
        public ProcessorChain ToChain(bool isOwned = true) => processor as ProcessorChain ?? new ProcessorChain(processor, isOwned);

        /// <summary>
        /// Mixes the <paramref name="other"/> provider with this one.
        /// </summary>
        /// <param name="other">The provider to add to the mixer anonymously.</param>
        /// <param name="isOtherOwned">Whether to dispose of <paramref name="other"/> when the mixer is disposed or when if input is removed.</param>
        /// <param name="isThisOwned">
        /// When creating a new <see cref="Mixer"/>,
        /// specifies whether to dispose of the current provider when the mixer is disposed or if the current input is removed.
        /// </param>
        /// <returns>The current provider as a <see cref="Mixer"/> or a new mixer with both providers added as inputs.</returns>
        public Mixer MixWith(ISampleProvider other, bool isOtherOwned = true, bool isThisOwned = true)
            => (processor as Mixer ?? new Mixer(processor, isThisOwned))
                .AddAnonymous(other, isOtherOwned);

        /// <summary>
        /// Processes the processor, given a <see cref="ModifyChain"/> delegate.
        /// </summary>
        /// <param name="process">The method to use to modify the chain.</param>
        /// <returns>The processor itself if <paramref name="process"/> is null, otherwise, a processed <see cref="ProcessorChain"/> with the source as the processor.</returns>
        public IAudioProcessor Process(ModifyChain? process)
        {
            if (process == null)
                return processor;
            var chain = processor.ToChain();
            process(chain);
            return chain;
        }

    }

    /// <param name="processor">The audio processor to extract from.</param>
    extension<T>(IAudioProcessor processor)
    {

        /// <summary>
        /// Attempts to get the source (original) processor.
        /// </summary>
        /// <param name="result">The found processor. <see langword="null"/> if no source was found.</param>
        /// <returns>Whether the source was found.</returns>
        public bool TryGetSourceAs([NotNullWhen(true)] out T? result)
        {
            switch (processor)
            {
                case ProcessorChain {Source: T t}:
                    result = t;
                    return true;
                case ProcessorChain {Source: IAudioProcessor source}:
                    return source.TryGetSourceAs(out result);
                case Mixer mixer when mixer.TryGetSingleMixerInput(out IAudioProcessor? mixerProcessor):
                    return mixerProcessor.TryGetSourceAs(out result);
                case Mixer mixer:
                    return mixer.TryGetSingleMixerInput(out result);
                case T t:
                    result = t;
                    return true;
                default:
                    result = default;
                    return false;
            }
        }

        public bool TryGetMasterAs([NotNullWhen(true)] out T? result)
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
                case Mixer mixer when mixer.TryGetSingleMixerInput(out IAudioProcessor? mixerProcessor):
                    return mixerProcessor.TryGetMasterAs(out result);
                case Mixer mixer:
                    return mixer.TryGetSingleMixerInput(out result);
                default:
                    result = default;
                    return false;
            }
        }

        public bool TryGetSingleMixerInput([NotNullWhen(true)] out T? result)
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

    }

}
