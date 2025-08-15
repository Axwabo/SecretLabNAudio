namespace SecretLabNAudio.Core.Providers;

/// <summary>A sample provider reading from a float array.</summary>
public sealed class RawSourceSampleProvider : ISampleProvider
{

    private readonly float[] _samples;

    /// <summary>The count of readable samples.</summary>
    public int Length { get; }

    /// <summary>The duration of readable samples as a <see cref="TimeSpan"/>.</summary>
    public TimeSpan TotalTime { get; }

    /// <summary>The current position of the provider.</summary>
    public int Position { get; set; }

    /// <summary>The current position of the provider as a <see cref="TimeSpan"/>.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a negative value or a value greater than <see cref="TotalTime"/>.</exception>
    public TimeSpan CurrentTime
    {
        get => TimeSpan.FromSeconds(WaveFormat.Seconds(Position));
        set
        {
            if (value < TimeSpan.Zero || value > TotalTime)
                throw new ArgumentOutOfRangeException(nameof(value), "Time must be within the range of the sample provider.");
            Position = WaveFormat.SampleCount(value.TotalSeconds);
        }
    }

    /// <summary>Creates a new <see cref="RawSourceSampleProvider"/> with the full sample array.</summary>
    /// <param name="samples">The samples to read from.</param>
    /// <param name="format">The <see cref="WaveFormat"/> of the samples.</param>
    /// <remarks>The <paramref name="format"/> is used to determine the rate and channels, from which an IEEEFloat format is created.</remarks>
    public RawSourceSampleProvider(float[] samples, WaveFormat format) : this(samples, samples.Length, format)
    {
    }

    /// <summary>Creates a new <see cref="RawSourceSampleProvider"/> with the specified length.</summary>
    /// <param name="samples">The samples to read from.</param>
    /// <param name="length">The number of samples to read.</param>
    /// <param name="format">The <see cref="WaveFormat"/> of the samples.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the length is greater than the number of samples provided or if it is negative.</exception>
    /// <remarks>The <paramref name="format"/> is used to determine the rate and channels, from which an IEEEFloat format is created.</remarks>
    public RawSourceSampleProvider(float[] samples, int length, WaveFormat format) : this(samples, length, format.SampleRate, format.Channels)
    {
    }

    /// <summary>Creates a new <see cref="RawSourceSampleProvider"/> with the full sample array.</summary>
    /// <param name="samples">The samples to read from.</param>
    /// <param name="sampleRate">The sample rate of the audio.</param>
    /// <param name="channels">The number of audio channels.</param>
    public RawSourceSampleProvider(float[] samples, int sampleRate, int channels) : this(samples, samples.Length, sampleRate, channels)
    {
    }

    /// <summary>Creates a new <see cref="RawSourceSampleProvider"/> with the specified length.</summary>
    /// <param name="samples">The samples to read from.</param>
    /// <param name="length">The number of samples to read.</param>
    /// <param name="sampleRate">The sample rate of the audio.</param>
    /// <param name="channels">The number of audio channels.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the length is greater than the number of samples provided or if it is negative.</exception>
    public RawSourceSampleProvider(float[] samples, int length, int sampleRate, int channels)
    {
        if (length > samples.Length)
            throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be greater than the number of samples provided.");
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative.");
        _samples = samples;
        Length = length;
        TotalTime = TimeSpan.FromSeconds(length);
        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
    }

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; }

    /// <inheritdoc/>
    public int Read(float[] buffer, int offset, int count)
    {
        var target = Mathf.Clamp(Length - Position, 0, count);
        if (target == 0)
            return 0;
        Array.Copy(_samples, Position, buffer, offset, target);
        Position += target;
        return target;
    }

}
