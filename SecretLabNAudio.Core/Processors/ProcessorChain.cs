using SecretLabNAudio.Core.Extensions.Processors;

namespace SecretLabNAudio.Core.Processors;

/// <summary>
/// A delegate to create a new <see cref="ISampleProvider"/> based on the current one or return the given provider itself,
/// </summary>
/// <param name="provider">The provider to map.</param>
public delegate ISampleProvider ProviderMapper(ISampleProvider provider);

public sealed class ProcessorChain : IAudioProcessor
{

    private readonly List<ProcessorLayer> _layers;

    public ISampleProvider Source
    {
        get
        {
            EnsureNotDisposed();
            return field;
        }
    }

    public ISampleProvider Master
    {
        get
        {
            EnsureNotDisposed();
            return _layers[^1].Provider;
        }
    }

    public IReadOnlyList<ProcessorLayer> Layers => _layers.AsReadOnly();

    public ProcessorChain(ISampleProvider source, bool isOwned = true)
    {
        Source = source;
        _layers = [new ProcessorLayer(source, isOwned)];
    }

    /// <inheritdoc />
    public WaveFormat WaveFormat => Master.WaveFormat;

    /// <inheritdoc />
    public int Read(float[] buffer, int offset, int count) => Master.Read(buffer, offset, count);

    private void EnsureNotDisposed()
    {
        if (_layers.Count == 0)
            throw new ObjectDisposedException(nameof(ProcessorChain));
    }

    public ProcessorChain Layer(ProviderMapper mapper, bool isOwned = true)
    {
        var master = Master;
        var converted = mapper(master);
        if (master == converted)
            return this;
        _layers.Add(new ProcessorLayer(converted, isOwned));
        return this;
    }

    public ProcessorChain Pop()
    {
        if (_layers.Count < 2)
            return this;
        _layers[^1].Dispose();
        _layers.RemoveAt(_layers.Count - 1);
        return this;
    }

    public void PopAll()
    {
        if (_layers.Count < 2)
            return;
        for (var i = 1; i < _layers.Count; i++)
            _layers[i].Dispose();
        _layers.RemoveRange(1, _layers.Count - 2);
    }

    /// <inheritdoc />
    public void Dispose() => _layers.DisposeAllAndClear();

}
