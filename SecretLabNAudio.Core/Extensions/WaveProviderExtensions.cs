using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="IWaveProvider"/> interface.</summary>
public static class WaveProviderExtensions
{

    /// <param name="waveProvider">Wave provider to convert.</param>
    extension(IWaveProvider waveProvider)
    {

        /// <summary>
        /// Converts the wave provider to an <see cref="AudioPlayer"/>-compatible <see cref="ISampleProvider"/>.
        /// </summary>
        /// <returns>An <see cref="ISampleProvider"/> that is compatible with the <see cref="AudioPlayer"/>.</returns>
        /// <seealso cref="SampleProviderExtensions.ToPlayerCompatible"/>
        public ISampleProvider ToPlayerCompatible()
            => waveProvider.ToSampleProvider().ToPlayerCompatible();

        public IAudioProcessor ToCompatibleProcessor()
            => waveProvider is not WaveStream stream
                ? new SampleProviderWrapper(waveProvider.ToSampleProvider()).ToPlayerCompatible()
                : new StreamAudioProcessor(stream).ToPlayerCompatible();

        public ProcessorChain ToCompatibleChain() => waveProvider.ToCompatibleProcessor().ToChain();

    }

}
