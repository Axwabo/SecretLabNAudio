using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="RawSourceSampleProvider"/> class.</summary>ű
public static class RawSampleProviderExtensions
{

    /// <param name="provider">The <see cref="RawSourceSampleProvider"/> to restart.</param>
    extension(RawSourceSampleProvider provider)
    {

        /// <summary>Sets the provider's position to 0.</summary>
        /// <returns>The <see cref="RawSourceSampleProvider"/> itself.</returns>
        public RawSourceSampleProvider Restart()
        {
            provider.Position = 0;
            return provider;
        }

        /// <summary>Sets the provider's position to the specified sample count.</summary>
        /// <param name="position">The sample count to seek to.</param>
        /// <returns>The <see cref="RawSourceSampleProvider"/> itself.</returns>
        public RawSourceSampleProvider Seek(int position)
        {
            if (position < 0 || position >= provider.Length)
                throw new ArgumentOutOfRangeException(nameof(position), "Position must be within the range of the sample provider's length.");
            provider.Position = position;
            return provider;
        }

        /// <summary>Sets the provider's position to the specified time in seconds.</summary>
        /// <param name="seconds">The time in seconds to seek to.</param>
        /// <returns>The <see cref="RawSourceSampleProvider"/> itself.</returns>
        public RawSourceSampleProvider Seek(double seconds)
            => provider.Seek(provider.WaveFormat.SampleCount(seconds));

        /// <summary>Loops the given <see cref="RawSourceSampleProvider"/>.</summary>
        /// <returns>A new <see cref="LoopingRawSampleProvider"/> that wraps the given provider.</returns>
        [Obsolete($"Use {nameof(WithLoop)} instead.", true)]
        public LoopingRawSampleProvider Loop() => new(provider);

        /// <summary>
        /// Sets the <see cref="RawSourceSampleProvider.Loop"/> property.
        /// </summary>
        /// <param name="loop">Whether to loop the provider.</param>
        /// <returns>The <see cref="RawSourceSampleProvider"/> itself.</returns>
        public RawSourceSampleProvider WithLoop(bool loop = true)
        {
            provider.Loop = loop;
            return provider;
        }

    }

}
