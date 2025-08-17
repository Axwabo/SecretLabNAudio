using NAudio.Wave.SampleProviders;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="ISampleProvider"/> interface.</summary>
public static class SampleProviderExtensions
{

    /// <summary>Converts the provider to be compatible with <see cref="AudioPlayer"/>s.</summary>
    /// <param name="provider">The sample provider to convert.</param>
    /// <returns>The converted provider.</returns>
    /// <exception cref="ArgumentException">Thrown if the format's encoding is not IEEEFloat.</exception>
    /// <remarks>The method first mixes down to mono (if necessary), then resamples (if necessary).
    /// If the format is already compatible, the original <paramref name="provider"/> is returned.</remarks>
    /// <seealso cref="AudioPlayer.SupportedFormat"/>
    public static ISampleProvider ToPlayerCompatible(this ISampleProvider provider)
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
    /// <param name="current">The current sample provider.</param>
    /// <param name="other">The other sample provider to mix with.</param>
    /// <returns><paramref name="current"/> if it's a <see cref="MixingSampleProvider"/>, otherwise, a new one containing both providers.</returns>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="other"/> provider's format does not match that of <paramref name="current"/>.</exception>
    /// <remarks>This method returns <paramref name="current"/> if it's a <see cref="MixingSampleProvider"/>.</remarks>
    public static MixingSampleProvider MixWith(this ISampleProvider current, ISampleProvider other)
    {
        if (current is not MixingSampleProvider mixing)
            return new MixingSampleProvider([current, other]);
        mixing.AddMixerInput(other);
        return mixing;
    }

    /// <summary>Buffers the given sample provider by the specified amount of seconds.</summary>
    /// <param name="provider">The sample provider to buffer.</param>
    /// <param name="seconds">The number of seconds to buffer.</param>
    /// <returns>A new <see cref="BufferedSampleProvider"/> that buffers the given provider.</returns>
    /// <seealso cref="BufferedSampleProvider"/>
    public static BufferedSampleProvider Buffer(this ISampleProvider provider, double seconds) => new(provider, seconds);

    /// <summary>Queues the <paramref name="other"/> sample provider after <paramref name="provider"/>.</summary>
    /// <param name="provider">The sample provider to queue after.</param>
    /// <param name="other">The sample provider to queue.</param>
    /// <returns>A <see cref="SampleProviderQueue"/> containing both providers.</returns>
    /// <remarks>The <paramref name="provider"/> is reused if it's already a <see cref="SampleProviderQueue"/>.</remarks>
    public static SampleProviderQueue Queue(this ISampleProvider provider, ISampleProvider other)
    {
        var queue = provider as SampleProviderQueue ?? new SampleProviderQueue(provider.WaveFormat);
        queue.Enqueue(other);
        return queue;
    }

    /// <summary>Sets the volume of the sample provider.</summary>
    /// <param name="provider">The provider to change the volume of.</param>
    /// <param name="volume">The volume to set.</param>
    /// <returns>The original or a new <see cref="VolumeSampleProvider"/> with the specified volume.</returns>
    /// <remarks>This method returns the <paramref name="provider"/> itself if it's a <see cref="VolumeSampleProvider"/>.</remarks>
    public static VolumeSampleProvider Volume(this ISampleProvider provider, float volume = 1)
    {
        if (provider is not VolumeSampleProvider volumeProvider)
            return new VolumeSampleProvider(provider) {Volume = volume};
        volumeProvider.Volume = volume;
        return volumeProvider;
    }

}
