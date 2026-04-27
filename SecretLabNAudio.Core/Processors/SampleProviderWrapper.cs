namespace SecretLabNAudio.Core.Processors;

/// <summary>
/// A simple wrapper for the <see cref="ISampleProvider"/> class, optionally disposing of the related resource.
/// </summary>
public sealed class SampleProviderWrapper : IAudioProcessor
{

    internal ISampleProvider? Source;
    private IDisposable? _disposable;

    /// <summary>
    /// Creates a new <see cref="SampleProviderWrapper"/>.
    /// </summary>
    /// <param name="source">The sample provider to wrap.</param>
    /// <param name="disposable">The object to dispose. May be equivalent to <paramref name="source"/>.</param>
    public SampleProviderWrapper(ISampleProvider source, IDisposable? disposable = null)
    {
        Source = source;
        _disposable = disposable;
    }

    /// <inheritdoc/>
    public WaveFormat WaveFormat => Source?.WaveFormat ?? throw new ObjectDisposedException(nameof(SampleProviderWrapper));

    /// <inheritdoc/>
    public int Read(float[] buffer, int offset, int count) => Source?.Read(buffer, offset, count) ?? throw new ObjectDisposedException(nameof(SampleProviderWrapper));

    /// <inheritdoc/>
    public void Dispose()
    {
        Source = null;
        _disposable?.Dispose();
        _disposable = null;
    }

}
