using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace SecretLabNAudio.Core.Extensions;

public static class SampleProviderExtensions
{

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

    public static MixingSampleProvider MixWith(this ISampleProvider current, ISampleProvider other)
    {
        if (current is not MixingSampleProvider mixing)
            return new MixingSampleProvider([current, other]);
        mixing.AddMixerInput(other);
        return mixing;
    }

}
