using NAudio.Dsp;

namespace SecretLabNAudio.Core.Providers;

/// <summary>
/// A sample provider that stretches the input using a <see cref="WdlResampler"/>.
/// Both tempo and pitch are affected.
/// </summary>
public sealed class SpeedChangingSampleProvider : ISampleProvider
{

    private readonly ISampleProvider _source;
    private readonly int _sampleRate;
    private readonly int _channels;

    private readonly WdlResampler _resampler;

    private float _previousSpeed = 1;

    /// <summary>
    /// Creates a new <see cref="SpeedChangingSampleProvider"/>.
    /// </summary>
    /// <param name="source">The provider to read from.</param>
    public SpeedChangingSampleProvider(ISampleProvider source)
    {
        _source = source;
        _sampleRate = source.WaveFormat.SampleRate;
        _channels = source.WaveFormat.Channels;
        _resampler = new WdlResampler();
        _resampler.SetMode(true, 2, false);
        _resampler.SetFilterParms();
        _resampler.SetFeedMode(false);
        _resampler.SetRates(_sampleRate, _sampleRate);
    }

    /// <summary>
    /// The speed scalar, validated to be at least 0.
    /// A speed of 0 produces constant silence.
    /// A speed of 1 produces output directly from the underlying provider. 
    /// </summary>
    public float Speed
    {
        get;
        set => field = Mathf.Max(0, value);
    } = 1;

    /// <inheritdoc/>
    public WaveFormat WaveFormat => _source.WaveFormat;

    /// <inheritdoc/>
    public int Read(float[] buffer, int offset, int count)
    {
        if (Mathf.Approximately(Speed, 0))
        {
            Array.Clear(buffer, offset, count);
            return count;
        }

        if (Mathf.Approximately(Speed, 1))
            return _source.Read(buffer, offset, count);

        if (!Mathf.Approximately(_previousSpeed, Speed))
            _resampler.SetRates(_sampleRate * Speed, _sampleRate); // we lie to the resampler (actually peak solution)

        _previousSpeed = Speed;
        var samplesPerChannel = count / _channels;
        var targetCount = _resampler.ResamplePrepare(samplesPerChannel, _channels, out var inputBuffer, out var inputBufferOffset);
        var actualRead = _source.Read(inputBuffer, inputBufferOffset, targetCount * _channels) / _channels;
        return _resampler.ResampleOut(buffer, offset, actualRead, samplesPerChannel, _channels) * _channels;
    }

}
