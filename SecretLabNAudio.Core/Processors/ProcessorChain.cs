using System.Collections.Generic;

namespace SecretLabNAudio.Core.Processors;

public sealed class ProcessorChain : IAudioProcessor
{

    private readonly List<ProcessorLayer> _chain;

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
            return _chain[^1].Provider;
        }
    }

    public IReadOnlyList<ProcessorLayer> Chain => _chain.AsReadOnly();

    public ProcessorChain(ISampleProvider root, bool isOwned = true)
    {
        Root = root;
        _chain = [new ProcessorLayer(root, isOwned)];
    }

    /// <inheritdoc />
    public WaveFormat WaveFormat => Master.WaveFormat;

    private void EnsureNotDisposed()
    {
        if (_chain.Count == 0)
            throw new ObjectDisposedException(nameof(ProcessorChain));
    }

    public ProcessorChain Layer(Func<ISampleProvider, ISampleProvider> convert, bool isOwned = true)
    {
        var master = Master;
        var converted = convert(master);
        if (master == converted)
            return this;
        _chain.Add(new ProcessorLayer(converted, isOwned));
        return this;
    }

    public ProcessorChain Swap(Func<ISampleProvider, ISampleProvider> convert, bool isOwned = true)
    {
        EnsureNotDisposed();
        return Pop().Layer(convert, isOwned);
    }

    public ProcessorChain Pop()
    {
        if (_chain.Count < 2)
            return this;
        _chain[^1].Dispose();
        _chain.RemoveAt(_chain.Count - 1);
        return this;
    }

    /// <inheritdoc />
    public int Read(float[] buffer, int offset, int count) => Master.Read(buffer, offset, count);

    /// <inheritdoc />
    public void Dispose() => _chain.DisposeAllAndClear();

}
