namespace SecretLabNAudio.Core.Providers;

/// <summary>Wraps a <see cref="RawSourceSampleProvider"/> and restart it when reaching the end.</summary>
public sealed class LoopingRawSampleProvider : ISampleProvider
{

    /// <summary>The <see cref="RawSourceSampleProvider"/> to loop.</summary>
    public RawSourceSampleProvider Provider { get; }

    /// <summary>Creates a new <see cref="LoopingRawSampleProvider"/>.</summary>
    /// <param name="provider">The <see cref="RawSourceSampleProvider"/> to loop.</param>
    public LoopingRawSampleProvider(RawSourceSampleProvider provider) => Provider = provider;

    /// <inheritdoc />
    public WaveFormat WaveFormat => Provider.WaveFormat;

    /// <inheritdoc />
    public int Read(float[] buffer, int offset, int count)
    {
        var total = 0;
        while (total < count)
            total += Provider.Read(buffer, offset + total, count);
        return total;
    }

}
