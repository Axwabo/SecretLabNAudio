using NAudio.Wave;

namespace SecretLabNAudio.Core.Providers;

public sealed class RawSourceSampleProvider : ISampleProvider
{

    private readonly float[] _samples;

    public int Length { get; }

    public int Position { get; set; }

    public RawSourceSampleProvider(float[] samples, int sampleRate, int channels) : this(samples, samples.Length, sampleRate, channels)
    {
    }

    public RawSourceSampleProvider(float[] samples, int length, int sampleRate, int channels)
    {
        _samples = samples;
        Length = length;
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
    }

    public WaveFormat WaveFormat { get; }

    public int Read(float[] buffer, int offset, int count)
    {
        var target = Mathf.Clamp(Length - Position, 0, count);
        if (target == 0)
            return 0;
        Position += target;
        Array.Copy(_samples, Position - target, buffer, offset, target);
        return target;
    }

}
