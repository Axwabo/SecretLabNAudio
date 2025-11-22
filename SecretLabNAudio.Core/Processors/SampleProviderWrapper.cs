namespace SecretLabNAudio.Core.Processors;

public class SampleProviderWrapper : IAudioProcessor
{

    private ISampleProvider? _provider;
    private IDisposable? _disposable;

    public SampleProviderWrapper(ISampleProvider provider, IDisposable? disposable = null)
    {
        _provider = provider;
        _disposable = disposable;
    }

    /// <inheritdoc />
    public int Read(float[] buffer, int offset, int count) => _provider != null
        ? ReadFromProvider(_provider, buffer, offset, count)
        : throw new ObjectDisposedException(nameof(SampleProviderWrapper));

    protected virtual int ReadFromProvider(ISampleProvider provider, float[] buffer, int offset, int count)
        => provider.Read(buffer, offset, count);

    /// <inheritdoc />
    public WaveFormat WaveFormat => _provider?.WaveFormat ?? throw new ObjectDisposedException(nameof(SampleProviderWrapper));

    /// <inheritdoc />
    public void Dispose()
    {
        _provider = null;
        _disposable?.Dispose();
        _disposable = null;
    }

}
