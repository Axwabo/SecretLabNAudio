using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Processors;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="ISampleProvider"/> interface.</summary>
public static class SampleProviderExtensions
{

    /// <param name="provider">The sample provider to convert.</param>
    extension(ISampleProvider provider)
    {

        public IAudioProcessor ToCompatibleProcessor(bool isOwned = true)
            => (provider as IAudioProcessor ?? new SampleProviderWrapper(provider)).ToPlayerCompatible(isOwned);

        public ProcessorChain ToCompatibleChain(bool isOwned = true)
            => provider.ToCompatibleProcessor(isOwned).ToChain(isOwned);

    }

    /// <param name="provider">The sample provider to convert.</param>
    extension(ISampleProvider provider)
    {

        /// <summary>Converts the provider to be compatible with <see cref="AudioPlayer"/>s.</summary>
        /// <returns>The converted provider.</returns>
        /// <exception cref="ArgumentException">Thrown if the format's encoding is not IEEEFloat.</exception>
        /// <remarks>The method first mixes down to mono (if necessary), then resamples (if necessary).
        /// If the format is already compatible, the original <paramref name="provider"/> is returned.</remarks>
        /// <seealso cref="AudioPlayer.SupportedFormat"/>
        public ISampleProvider ToPlayerCompatible()
        {
            if (provider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                throw new ArgumentException($"Expected an IEEEFloat sample provider, got encoding {provider.WaveFormat.Encoding}");
            if (provider.WaveFormat.Channels != 1)
                provider = new StereoToMonoSampleProvider(provider);
            if (provider.WaveFormat.SampleRate != AudioPlayer.SampleRate)
                provider = new WdlResamplingSampleProvider(provider, AudioPlayer.SampleRate);
            return provider;
        }

        /// <summary>Mixes two sample providers.</summary>
        /// <param name="other">The other sample provider to mix with.</param>
        /// <returns><paramref name="provider"/> if it's a <see cref="MixingSampleProvider"/>, otherwise, a new one containing both providers.</returns>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="other"/> provider's format does not match that of <paramref name="provider"/>.</exception>
        /// <remarks>This method returns <paramref name="provider"/> if it's a <see cref="MixingSampleProvider"/>.</remarks>
        public MixingSampleProvider MixWith(ISampleProvider other)
        {
            if (provider is not MixingSampleProvider mixing)
                return new MixingSampleProvider([provider, other]);
            mixing.AddMixerInput(other);
            return mixing;
        }

        /// <summary>Buffers the given sample provider by the specified amount of seconds.</summary>
        /// <param name="seconds">The number of seconds to buffer.</param>
        /// <returns>A new <see cref="BufferedSampleProvider"/> that buffers the given provider.</returns>
        /// <seealso cref="BufferedSampleProvider"/>
        [Obsolete("", true)] // TODO: add error message
        public BufferedSampleProvider Buffer(double seconds) => new(provider, seconds);

        /// <summary>Queues the <paramref name="other"/> sample provider after <paramref name="provider"/>.</summary>
        /// <param name="other">The sample provider to queue.</param>
        /// <returns>A <see cref="SampleProviderQueue"/> containing both providers.</returns>
        /// <remarks>The <paramref name="provider"/> is reused if it's already a <see cref="SampleProviderQueue"/>.</remarks>
        [Obsolete("", true)] // TODO: add error message
        public SampleProviderQueue Queue(ISampleProvider other)
        {
            var queue = provider as SampleProviderQueue ?? new SampleProviderQueue(provider.WaveFormat);
            queue.Enqueue(other);
            return queue;
        }

        /// <summary>Sets the volume of the sample provider.</summary>
        /// <param name="volume">The volume to set.</param>
        /// <returns>The original or a new <see cref="VolumeSampleProvider"/> with the specified volume.</returns>
        /// <remarks>This method returns the <paramref name="provider"/> itself if it's a <see cref="VolumeSampleProvider"/>.</remarks>
        public VolumeSampleProvider Volume(float volume = 1)
        {
            if (provider is not VolumeSampleProvider volumeProvider)
                return new VolumeSampleProvider(provider) {Volume = volume};
            volumeProvider.Volume = volume;
            return volumeProvider;
        }

    }

}
