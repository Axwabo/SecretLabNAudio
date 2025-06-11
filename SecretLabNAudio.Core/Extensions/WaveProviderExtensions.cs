namespace SecretLabNAudio.Core.Extensions;

public static class WaveProviderExtensions
{

    public static ISampleProvider ToPlayerCompatible(this IWaveProvider waveProvider)
        => waveProvider.ToSampleProvider().ToPlayerCompatible();

}
