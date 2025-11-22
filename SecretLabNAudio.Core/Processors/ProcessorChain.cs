using System.Collections.Generic;

namespace SecretLabNAudio.Core.Processors;

public delegate ISampleProvider ProviderMapper(ISampleProvider provider);

public sealed class ProcessorChain : IAudioProcessor
{

    private readonly List<ProcessorLayer> _layers;

    public ISampleProvider Root
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

    public ProcessorChain(ISampleProvider root, bool isOwned = true)
    {
        Root = root;
        _layers = [new ProcessorLayer(root, isOwned)];
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

    public ProcessorChain Swap(ProviderMapper mapper, bool isOwned = true)
    {
        EnsureNotDisposed();
        return Pop().Layer(mapper, isOwned);
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
        for (var i = 1; i < _layers.Count; i++)
            _layers[i].Dispose();
        _layers.RemoveRange(1, _layers.Count - 2);
    }

    /// <inheritdoc />
    public void Dispose() => _layers.DisposeAllAndClear();

}
