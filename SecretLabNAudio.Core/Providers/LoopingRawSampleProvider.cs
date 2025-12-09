namespace SecretLabNAudio.Core.Providers;

/// <summary>Wraps a <see cref="RawSourceSampleProvider"/> and restart it when reaching the end.</summary>
[Obsolete($"Use {nameof(RawSourceSampleProvider)} instead.", true)]
public sealed class LoopingRawSampleProvider : ISampleProvider, ISeekable, ILoopable
{

    /// <summary>The <see cref="RawSourceSampleProvider"/> to loop.</summary>
    public RawSourceSampleProvider Provider { get; }

    /// <summary>Creates a new <see cref="LoopingRawSampleProvider"/>.</summary>
    /// <param name="provider">The <see cref="RawSourceSampleProvider"/> to loop.</param>
    public LoopingRawSampleProvider(RawSourceSampleProvider provider) => Provider = provider;

    /// <inheritdoc/>
    public WaveFormat WaveFormat => Provider.WaveFormat;

    /// <inheritdoc />
    public TimeSpan CurrentTime
    {
        get => Provider.CurrentTime;
        set => Provider.CurrentTime = value;
    }

    /// <inheritdoc />
    public TimeSpan TotalTime => Provider.TotalTime;

    /// <inheritdoc />
    public bool Loop { get; set; } = true;

    /// <inheritdoc/>
    public int Read(float[] buffer, int offset, int count)
    {
        if (!Loop)
            return Provider.Read(buffer, offset, count);
        var total = 0;
        while (total < count)
        {
            var target = count - total;
            var read = Provider.Read(buffer, offset + total, target);
            if (read < target)
                Provider.Position = 0;
            total += read;
        }

        return total;
    }

}
