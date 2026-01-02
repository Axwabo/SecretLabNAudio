using SecretLabNAudio.Core.Extensions.Processors;

namespace SecretLabNAudio.Core.Processors;

/// <summary>
/// A delegate to create a new <see cref="ISampleProvider"/> based on the current one or return the given provider itself.
/// </summary>
/// <param name="provider">The provider to map.</param>
public delegate ISampleProvider ProviderMapper(ISampleProvider provider);

/// <summary>
/// An audio processor that lets you build a chain of providers, allowing for adding and removing and iteration of the layers.
/// </summary>
public sealed class ProcessorChain : IAudioProcessor
{

    private readonly List<ProcessorLayer> _layers;

    /// <summary>The original provider this chain was created with.</summary>
    public ISampleProvider Source
    {
        get
        {
            EnsureNotDisposed();
            return field;
        }
    }

    /// <summary>The last layer that will be read from. Equivalent to <see cref="Source"/> if no layers were added.</summary>
    public ISampleProvider Master
    {
        get
        {
            EnsureNotDisposed();
            return _layers[^1].Provider;
        }
    }

    /// <summary>
    /// The list of layers, beginning with <see cref="Source"/>.
    /// If there are intermediate layers, those are in the order they were added in.
    /// </summary>
    public IReadOnlyList<ProcessorLayer> Layers { get; }

    /// <summary>
    /// Creates a new <see cref="ProcessorChain"/>.
    /// </summary>
    /// <param name="source">The source layer to read from.</param>
    /// <param name="isOwned">Whether to dispose of <paramref name="source"/> when this chain is disposed.</param>
    public ProcessorChain(ISampleProvider source, bool isOwned = true)
    {
        Source = source;
        _layers = [new ProcessorLayer(source, isOwned)];
        Layers = _layers.AsReadOnly();
    }

    /// <inheritdoc />
    public WaveFormat WaveFormat => Master.WaveFormat;

    /// <inheritdoc />
    public int Read(float[] buffer, int offset, int count) => Master.Read(buffer, offset, count);

    /// <summary>
    /// Adds a new layer to the end of the chain by mapping the <see cref="Master"/> provider.
    /// </summary>
    /// <param name="mapper">The delegate to use to convert the provider.</param>
    /// <param name="isOwned">Whether to dispose of the new layer when this chain is disposed or if the layer is removed.</param>
    /// <returns>The chain itself.</returns>
    /// <remarks>No layer is added if the mapper returns the <see cref="Master"/> directly.</remarks>
    public ProcessorChain Layer(ProviderMapper mapper, bool isOwned = true)
    {
        var master = Master;
        var converted = mapper(master);
        if (master == converted)
            return this;
        _layers.Add(new ProcessorLayer(converted, isOwned));
        return this;
    }

    /// <summary>
    /// Removes the last layer (if it's not the source).
    /// </summary>
    /// <returns>The chain itself.</returns>
    public ProcessorChain Pop()
    {
        if (_layers.Count < 2)
            return this;
        _layers[^1].Dispose();
        _layers.RemoveAt(_layers.Count - 1);
        return this;
    }

    /// <summary>
    /// Removes all layers (except the source).
    /// </summary>
    public ProcessorChain PopAll()
    {
        if (_layers.Count < 2)
            return this;
        for (var i = 1; i < _layers.Count; i++)
            _layers[i].Dispose();
        _layers.RemoveRange(1, _layers.Count - 2);
        return this;
    }

    /// <inheritdoc />
    public void Dispose() => _layers.DisposeAllAndClear();

    private void EnsureNotDisposed()
    {
        if (_layers.Count == 0)
            throw new ObjectDisposedException(nameof(ProcessorChain));
    }

}
