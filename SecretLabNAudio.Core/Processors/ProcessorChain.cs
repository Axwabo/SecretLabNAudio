using System.Collections.Generic;

namespace SecretLabNAudio.Core.Processors;

public sealed class ProcessorChain : IAudioProcessor
{

    public ISampleProvider Root
    {
        get
        {
            EnsureNotDisposed();
            return field;
        }
    }

    public ISampleProvider Last
    {
        get
        {
            EnsureNotDisposed();
            return _chain[^1].Provider;
        }
    }

    private readonly List<ProcessorInput> _chain;

    public IReadOnlyList<ProcessorInput> Chain => _chain.AsReadOnly();

    public ProcessorChain(ISampleProvider root, bool isOwned = true)
    {
        Root = root;
        _chain = [new ProcessorInput(root, isOwned)];
    }

    /// <inheritdoc />
    public WaveFormat WaveFormat => Last.WaveFormat;

    private void EnsureNotDisposed()
    {
        if (_chain.Count == 0)
            throw new ObjectDisposedException(nameof(ProcessorChain));
    }

    public ProcessorChain Layer(Func<ISampleProvider, ISampleProvider> convert, bool isOwned = true)
    {
        var last = Last;
        var converted = convert(last);
        if (last == converted)
            return this;
        _chain.Add(new ProcessorInput(converted, isOwned));
        return this;
    }

    public ProcessorChain Replace(Func<ISampleProvider, ISampleProvider> convert, bool isOwned = true)
    {
        EnsureNotDisposed();
        return Pop().Layer(convert, isOwned);
    }

    public ProcessorChain Pop()
    {
        if (_chain.Count < 2)
            return this;

        _chain.RemoveAt(_chain.Count - 1);
        return this;
    }

    /// <inheritdoc />
    public int Read(float[] buffer, int offset, int count) => Last.Read(buffer, offset, count);

    /// <inheritdoc />
    public void Dispose() => _chain.DisposeAllAndClear();

}
