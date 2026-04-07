using NAudio.Wave.SampleProviders;

namespace SecretLabNAudio.Core.Extensions.Providers;

/// <summary>
/// Extension methods for the <see cref="ISampleProvider"/> interface.
/// These methods are meant for providers that aren't audio processors.
/// </summary>
public static class NonProcessorExtensions
{

    /// <param name="provider">The wave provider to convert.</param>
    extension(IWaveProvider provider)
    {

        /// <summary>
        /// Converts the wave provider to an <see cref="AudioConstants"/>-compatible <see cref="ISampleProvider"/>.
        /// </summary>
        /// <returns>An <see cref="ISampleProvider"/> that is compatible with the <see cref="AudioConstants"/>.</returns>
        /// <seealso cref="SampleProviderExtensions.ToPlayerCompatible"/>
        public ISampleProvider ToPlayerCompatible() => provider.ToSampleProvider().ToPlayerCompatible();

    }

    /// <param name="provider">The sample provider to convert.</param>
    extension(ISampleProvider provider)
    {

        /// <summary>Converts the provider to be compatible with <see cref="AudioConstants"/>s.</summary>
        /// <returns>The converted provider.</returns>
        /// <exception cref="ArgumentException">Thrown if the format's encoding is not IEEEFloat.</exception>
        /// <remarks>The method first mixes down to mono (if necessary), then resamples (if necessary).
        /// If the format is already compatible, the original provider is returned.</remarks>
        /// <seealso cref="AudioConstants.SupportedFormat"/>
        public ISampleProvider ToPlayerCompatible()
        {
            if (provider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                throw new ArgumentException($"Expected an IEEEFloat sample provider, got encoding {provider.WaveFormat.Encoding}");
            if (provider.WaveFormat.Channels != 1)
                provider = new StereoToMonoSampleProvider(provider);
            if (provider.WaveFormat.SampleRate != AudioConstants.SampleRate)
                provider = new WdlResamplingSampleProvider(provider, AudioConstants.SampleRate);
            return provider;
        }

        /// <summary>Mixes two sample providers.</summary>
        /// <param name="other">The other sample provider to mix with.</param>
        /// <returns>provider if it's a <see cref="MixingSampleProvider"/>, otherwise, a new one containing both providers.</returns>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="other"/> provider's format does not match that of provider.</exception>
        /// <remarks>This method returns provider if it's a <see cref="MixingSampleProvider"/>.</remarks>
        public MixingSampleProvider MixWith(ISampleProvider other)
        {
            if (provider is not MixingSampleProvider mixing)
                return new MixingSampleProvider([provider, other]);
            mixing.AddMixerInput(other);
            return mixing;
        }

        /// <summary>Sets the volume of the sample provider.</summary>
        /// <param name="volume">The volume to set.</param>
        /// <returns>The original or a new <see cref="VolumeSampleProvider"/> with the specified volume.</returns>
        /// <remarks>This method returns the provider itself if it's a <see cref="VolumeSampleProvider"/>.</remarks>
        public VolumeSampleProvider Volume(float volume = 1)
        {
            if (provider is not VolumeSampleProvider volumeProvider)
                return new VolumeSampleProvider(provider) {Volume = volume};
            volumeProvider.Volume = volume;
            return volumeProvider;
        }

    }

}
