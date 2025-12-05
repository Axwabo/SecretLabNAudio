namespace SecretLabNAudio.Core.Processors;

/// <summary>
/// A simple wrapper for the <see cref="ISampleProvider"/> class, optionally disposing of the related resource.
/// </summary>
public sealed class SampleProviderWrapper : IAudioProcessor
{

    private ISampleProvider? _provider;
    private IDisposable? _disposable;

    /// <summary>
    /// Creates a new <see cref="SampleProviderWrapper"/>.
    /// </summary>
    /// <param name="provider">The sample provider to wrap.</param>
    /// <param name="disposable">The object to dispose. May be equivalent to <paramref name="provider"/>.</param>
    public SampleProviderWrapper(ISampleProvider provider, IDisposable? disposable = null)
    {
        _provider = provider;
        _disposable = disposable;
    }

    /// <inheritdoc />
    public WaveFormat WaveFormat => _provider?.WaveFormat ?? throw new ObjectDisposedException(nameof(SampleProviderWrapper));

    /// <inheritdoc />
    public int Read(float[] buffer, int offset, int count) => _provider?.Read(buffer, offset, count) ?? throw new ObjectDisposedException(nameof(SampleProviderWrapper));

    /// <inheritdoc />
    public void Dispose()
    {
        _provider = null;
        _disposable?.Dispose();
        _disposable = null;
    }

}
