using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Extensions.Providers;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="ISampleProvider"/> interface.</summary>
[Obsolete($"Moved to Providers.{nameof(NonProcessorExtensions)}", true)]
public static class SampleProviderExtensions
{

    /// <param name="provider">The sample provider to convert.</param>
    extension(ISampleProvider provider)
    {

        /// <inheritdoc cref="NonProcessorExtensions.ToPlayerCompatible(ISampleProvider)"/>
        [Obsolete($"Use Providers.{nameof(NonProcessorExtensions)}.{nameof(NonProcessorExtensions.ToPlayerCompatible)} instead.", true)]
        public ISampleProvider ToPlayerCompatible() => NonProcessorExtensions.ToPlayerCompatible(provider);

        /// <inheritdoc cref="NonProcessorExtensions.MixWith"/>
        [Obsolete($"Use Providers.{nameof(NonProcessorExtensions)}.{nameof(NonProcessorExtensions.MixWith)} instead.", true)]
        public MixingSampleProvider MixWith(ISampleProvider other) => NonProcessorExtensions.MixWith(provider, other);

        /// <summary>Buffers the given sample provider by the specified amount of seconds.</summary>
        /// <param name="seconds">The number of seconds to buffer.</param>
        /// <returns>A new <see cref="BufferedSampleProvider"/> that buffers the given provider.</returns>
        /// <seealso cref="BufferedSampleProvider"/>
        [Obsolete("Call the constructor instead.", true)]
        public BufferedSampleProvider Buffer(double seconds) => new(provider, seconds);

        /// <summary>Queues the <paramref name="other"/> sample provider after provider.</summary>
        /// <param name="other">The sample provider to queue.</param>
        /// <returns>A <see cref="SampleProviderQueue"/> containing both providers.</returns>
        /// <remarks>The provider is reused if it's already a <see cref="SampleProviderQueue"/>.</remarks>
        [Obsolete($"Prefer using the AudioQueue class. Safe cast or call the constructor instead, then invoke {nameof(SampleProviderQueue.Enqueue)}.", true)]
        public SampleProviderQueue Queue(ISampleProvider other)
        {
            var queue = provider as SampleProviderQueue ?? new SampleProviderQueue(provider.WaveFormat);
            queue.Enqueue(other);
            return queue;
        }

        /// <summary>Sets the volume of the sample provider.</summary>
        /// <param name="volume">The volume to set.</param>
        /// <returns>The original or a new <see cref="VolumeSampleProvider"/> with the specified volume.</returns>
        /// <remarks>This method returns the provider itself if it's a <see cref="VolumeSampleProvider"/>.</remarks>
        [Obsolete($"Use Providers.{nameof(NonProcessorExtensions)}.{nameof(NonProcessorExtensions.Volume)} instead.", true)]
        public VolumeSampleProvider Volume(float volume = 1)
        {
            if (provider is not VolumeSampleProvider volumeProvider)
                return new VolumeSampleProvider(provider) {Volume = volume};
            volumeProvider.Volume = volume;
            return volumeProvider;
        }

    }

}
