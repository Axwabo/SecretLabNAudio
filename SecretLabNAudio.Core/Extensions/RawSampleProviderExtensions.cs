using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="RawSourceSampleProvider"/> class.</summary>
public static class RawSampleProviderExtensions
{

    /// <summary>Sets the provider's position to 0.</summary>
    /// <param name="provider">The <see cref="RawSourceSampleProvider"/> to restart.</param>
    /// <returns>The <see cref="RawSourceSampleProvider"/> itself.</returns>
    public static RawSourceSampleProvider Restart(this RawSourceSampleProvider provider)
    {
        provider.Position = 0;
        return provider;
    }

    /// <summary>Sets the provider's position to the specified sample count.</summary>
    /// <param name="provider">The <see cref="RawSourceSampleProvider"/> to seek.</param>
    /// <param name="position">The sample count to seek to.</param>
    /// <returns>The <see cref="RawSourceSampleProvider"/> itself.</returns>
    public static RawSourceSampleProvider Seek(this RawSourceSampleProvider provider, int position)
    {
        if (position < 0 || position >= provider.Length)
            throw new ArgumentOutOfRangeException(nameof(position), "Position must be within the range of the sample provider's length.");
        provider.Position = position;
        return provider;
    }

    /// <summary>Sets the provider's position to the specified time in seconds.</summary>
    /// <param name="provider">The <see cref="RawSourceSampleProvider"/> to seek.</param>
    /// <param name="seconds">The time in seconds to seek to.</param>
    /// <returns>The <see cref="RawSourceSampleProvider"/> itself.</returns>
    public static RawSourceSampleProvider Seek(this RawSourceSampleProvider provider, double seconds)
        => provider.Seek(provider.WaveFormat.SampleCount(seconds));

    /// <summary>Loops the given <see cref="RawSourceSampleProvider"/>.</summary>
    /// <param name="provider">The <see cref="RawSourceSampleProvider"/> to loop.</param>
    /// <returns>A new <see cref="LoopingRawSampleProvider"/> that wraps the given provider.</returns>
    public static LoopingRawSampleProvider Loop(this RawSourceSampleProvider provider) => new(provider);

}
