namespace SecretLabNAudio.Core.Extensions.Processors;

/// <summary>Extension methods for the <see cref="IAudioProcessor"/> interface.</summary>
public static class AudioProcessorExtensions
{

    /// <param name="processor">The audio processor to modify.</param>
    extension(IAudioProcessor processor)
    {

        /// <summary>
        /// Mixes down and resamples the processor to be compatible with <see cref="AudioConstants.SupportedFormat"/> if needed.
        /// </summary>
        /// <param name="isOwned">Whether to dispose this processor when the newly created <see cref="ProcessorChain"/> is disposed.</param>
        /// <returns>The processor itself if the format is already compatible, otherwise, a player-compatible <see cref="ProcessorChain"/>.</returns>
        public IAudioProcessor ToPlayerCompatible(bool isOwned = true)
            => processor.WaveFormat.Matches(AudioConstants.SampleRate, AudioConstants.Channels)
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
    /// <typeparam name="T">The type of object to extract.</typeparam>
    extension<T>(IAudioProcessor processor)
    {

        /// <summary>
        /// Attempts to get the source (original) processor.
        /// </summary>
        /// <param name="result">The found processor. <see langword="null"/> if no source was found.</param>
        /// <returns>Whether the source was found.</returns>
        /// <remarks>
        /// The order of processor checks is as follows:
        /// <list type="number">
        /// <item><description>
        /// <see cref="ProcessorChain"/> with a <see cref="ProcessorChain.Source">source</see> of type <typeparamref name="T"/>
        /// = the source of the chain as <typeparamref name="T"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="ProcessorChain"/> with an <see cref="IAudioProcessor"/> <see cref="ProcessorChain.Source">source</see>
        /// = <see cref="TryGetSourceAs"/> called with the source processor
        /// </description></item>
        /// <item><description>
        /// <see cref="SampleProviderWrapper"/> with a source of type <typeparamref name="T"/>
        /// = the source of the wrapper as <typeparamref name="T"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="SampleProviderWrapper"/> with an <see cref="IAudioProcessor"/> source
        /// = <see cref="TryGetSourceAs"/> called with the source processor
        /// </description></item>
        /// <item><description>
        /// <see cref="Mixer"/> with a <see cref="TryGetSingleMixerInput">single input</see> of type <see cref="IAudioProcessor"/>
        /// = <see cref="TryGetSourceAs"/> called with the single input
        /// </description></item>
        /// <item><description><see cref="Mixer"/> = <see cref="TryGetSingleMixerInput"/> called with the mixer</description></item>
        /// <item><description>of type <typeparamref name="T"/> = processor as <typeparamref name="T"/></description></item>
        /// </list>
        /// </remarks>
        public bool TryGetSourceAs([NotNullWhen(true)] out T? result)
        {
            switch (processor)
            {
                case ProcessorChain {Source: T t}:
                    result = t;
                    return true;
                case ProcessorChain {Source: IAudioProcessor source}:
                    return source.TryGetSourceAs(out result);
                case SampleProviderWrapper {Source: T t}:
                    result = t;
                    return true;
                case SampleProviderWrapper {Source: IAudioProcessor source}:
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

        /// <summary>
        /// Attempts to get the master (final) processor.
        /// </summary>
        /// <param name="result">The found processor. <see langword="null"/> if no master was found.</param>
        /// <returns>Whether the source was found.</returns>
        /// <remarks>
        /// The order of processor checks is as follows:
        /// <list type="number">
        /// <item><description>of type <typeparamref name="T"/> = processor as <typeparamref name="T"/></description></item>
        /// <item><description>
        /// <see cref="ProcessorChain"/> with a <see cref="ProcessorChain.Master">master</see> of type <typeparamref name="T"/>
        /// = the master of the chain as <typeparamref name="T"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="ProcessorChain"/> with an <see cref="IAudioProcessor"/> <see cref="ProcessorChain.Master">master</see>
        /// = <see cref="TryGetMasterAs"/> called with the master processor
        /// </description></item>
        /// <item><description>
        /// <see cref="SampleProviderWrapper"/> with a source of type <typeparamref name="T"/>
        /// = the source of the wrapper as <typeparamref name="T"/>
        /// </description></item>
        /// <item><description>
        /// <see cref="SampleProviderWrapper"/> with an <see cref="IAudioProcessor"/> source
        /// = <see cref="TryGetMasterAs"/> called with the source processor
        /// </description></item>
        /// <item><description>
        /// <see cref="Mixer"/> with a <see cref="TryGetSingleMixerInput">single input</see> of type <see cref="IAudioProcessor"/>
        /// = <see cref="TryGetMasterAs"/> called with the single input
        /// </description></item>
        /// <item><description><see cref="Mixer"/> = <see cref="TryGetSingleMixerInput"/> called with the mixer</description></item>
        /// </list>
        /// </remarks>
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
                case SampleProviderWrapper {Source: T t}:
                    result = t;
                    return true;
                case SampleProviderWrapper {Source: IAudioProcessor source}:
                    return source.TryGetMasterAs(out result);
                case Mixer mixer when mixer.TryGetSingleMixerInput(out IAudioProcessor? mixerProcessor):
                    return mixerProcessor.TryGetMasterAs(out result);
                case Mixer mixer:
                    return mixer.TryGetSingleMixerInput(out result);
                default:
                    result = default;
                    return false;
            }
        }

        /// <summary>
        /// Attempts to get the single mixer input.
        /// </summary>
        /// <param name="result">The single input. <see langword="null"/> if there isn't exactly 1 input of type <typeparamref name="T"/>.</param>
        /// <returns>Whether there was exactly 1 input found.</returns>
        /// <remarks>
        /// This method does not directly check the processor against type <typeparamref name="T"/>.<br/>
        /// The order of processor checks is as follows:
        /// <list type="number">
        /// <item><description><see cref="Mixer"/> with exactly one input of type <typeparamref name="T"/> = the one input</description></item>
        /// <item><description><see cref="SampleProviderWrapper"/> with source of type <typeparamref name="T"/> = the source</description></item>
        /// <item><description>
        /// <see cref="SampleProviderWrapper"/> with an <see cref="IAudioProcessor"/> source
        /// = <see cref="TryGetSingleMixerInput"/> called with the source
        /// </description></item>
        /// <item><description><see cref="ProcessorChain"/> with <see cref="ProcessorChain.Source"/> of type <typeparamref name="T"/> = the source</description></item>
        /// <item><description>
        /// <see cref="ProcessorChain"/> with an <see cref="IAudioProcessor"/> <see cref="ProcessorChain.Source"/>
        /// = <see cref="TryGetSingleMixerInput"/> called with the source
        /// </description></item>
        /// </list>
        /// </remarks>
        public bool TryGetSingleMixerInput([NotNullWhen(true)] out T? result)
        {
            switch (processor)
            {
                case Mixer {Inputs: [{Provider: T t}]}:
                    result = t;
                    return true;
                case SampleProviderWrapper {Source: T t}:
                    result = t;
                    return true;
                case SampleProviderWrapper {Source: IAudioProcessor source}:
                    return source.TryGetSingleMixerInput(out result);
                case ProcessorChain {Source: T t}:
                    result = t;
                    return true;
                case ProcessorChain {Source: IAudioProcessor master}:
                    return master.TryGetSingleMixerInput(out result);
                default:
                    result = default;
                    return false;
            }
        }

    }

}
