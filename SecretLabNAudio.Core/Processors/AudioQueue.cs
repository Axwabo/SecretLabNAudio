using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Processors;

/// <summary>An audio processor reading from a queue of providers.</summary>
public sealed class AudioQueue : IAudioProcessor
{

    private readonly Queue<ProcessorLayer> _queue = [];

    private ProcessorLayer? _current;

    /// <summary>A read-only collection representing the underlying queue. Does not contain <seealso cref="Current"/>.</summary>
    public IReadOnlyCollection<ProcessorLayer> Queue => _queue;

    /// <summary>Gets the active <see cref="ProcessorLayer"/> (if any).</summary>
    public ProcessorLayer? CurrentLayer => _current;

    /// <summary>Gets the active provider (if any).</summary>
    public ISampleProvider? Current => _current?.Provider;

    /// <summary>Creates a new <see cref="SampleProviderQueue"/>.</summary>
    /// <param name="waveFormat">The <see cref="WaveFormat"/> of the audio.</param>
    public AudioQueue(WaveFormat waveFormat) => WaveFormat = waveFormat;

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; }

    /// <inheritdoc/>
    public int Read(float[] buffer, int offset, int count)
    {
        if (_current is null && !Next())
            return 0;
        var provider = _current!.Provider;
        var total = 0;
        while (total < count)
        {
            var target = count - total;
            var read = provider.Read(buffer, total, target);
            total += read;
            if (read < target && !Next())
                break;
        }

        return total;
    }

    /// <summary>Queues a provider to be read from.</summary>
    /// <param name="provider">The <see cref="ISampleProvider"/> to queue.</param>
    /// <exception cref="FormatException">Thrown if the provider's wave format does not match this provider's <see cref="WaveFormat"/>.</exception>
    public AudioQueue Enqueue(ISampleProvider provider) => Enqueue(provider, true);

    /// <summary>Queues a provider to be read from.</summary>
    /// <param name="provider">The <see cref="ISampleProvider"/> to queue.</param>
    /// <param name="isOwned">Whether to dispose of the <paramref name="provider"/> after it has ended or if this queue gets disposed.</param>
    /// <exception cref="FormatException">Thrown if the provider's wave format does not match this provider's <see cref="WaveFormat"/>.</exception>
    public AudioQueue Enqueue(ISampleProvider provider, bool isOwned)
    {
        if (!WaveFormat.Equals(provider.WaveFormat))
            throw new FormatException("The provider's wave format must match the queue's format.");
        _queue.Enqueue(new ProcessorLayer(provider, isOwned));
        return this;
    }

    /// <summary>Dequeues the next provider in the queue.</summary>
    /// <returns>True if a provider was dequeued, false if the queue is already empty.</returns>
    public bool Next()
    {
        _current?.Dispose();
        return _queue.TryDequeue(out _current);
    }

    /// <summary>Clears the queue and disposes of owned queued providers.</summary>
    /// <remarks>The current layer is kept active. Call <see cref="Next"/> to end it.</remarks>
    public void Clear()
    {
        foreach (var layer in _queue)
            layer.Dispose();
        _queue.Clear();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _current?.Dispose();
        Clear();
    }

}
