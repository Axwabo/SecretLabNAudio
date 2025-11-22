using System.Collections.Generic;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.Core.Providers;

/// <summary>A sample provider reading from a queue of providers.</summary>
public sealed class SampleProviderQueue : IAudioProcessor
{

    private readonly Queue<ProcessorLayer> _queue = [];

    private ProcessorLayer? _current;

    /// <summary>A read-only collection representing the underlying queue. Does not contain <seealso cref="Current"/>.</summary>
    public IReadOnlyCollection<ProcessorLayer> Queue => _queue;

    /// <summary>Gets the active provider (if any).</summary>
    public ISampleProvider? Current => _current?.Provider;

    /// <summary>Creates a new <see cref="SampleProviderQueue"/>.</summary>
    /// <param name="waveFormat">The <see cref="WaveFormat"/> of the audio.</param>
    public SampleProviderQueue(WaveFormat waveFormat) => WaveFormat = waveFormat;

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; }

    /// <inheritdoc/>
    public int Read(float[] buffer, int offset, int count)
    {
        // TODO: safely advance
        if (_current is null && !_queue.TryDequeue(out _current))
            return 0;
        var total = 0;
        while (total < count)
        {
            var target = count - total;
            var read = _current.Provider.Read(buffer, total, target);
            total += read;
            if (read < target && !Next())
                break;
        }

        return total;
    }

    /// <summary>Queues a provider to be read from.</summary>
    /// <param name="provider">The <see cref="ISampleProvider"/> to queue.</param>
    /// <exception cref="FormatException">Thrown if the provider's wave format does not match this provider's <see cref="WaveFormat"/>.</exception>
    public void Enqueue(ISampleProvider provider) => Enqueue(provider, true);

    /// <summary>Queues a provider to be read from.</summary>
    /// <param name="provider">The <see cref="ISampleProvider"/> to queue.</param>
    /// <param name="isOwned"></param>
    /// <exception cref="FormatException">Thrown if the provider's wave format does not match this provider's <see cref="WaveFormat"/>.</exception>
    public void Enqueue(ISampleProvider provider, bool isOwned)
    {
        if (!WaveFormat.Equals(provider.WaveFormat))
            throw new FormatException("The provider's wave format must match the queue's format.");
        _queue.Enqueue(new ProcessorLayer(provider, isOwned));
    }

    /// <summary>Dequeues the next provider in the queue.</summary>
    /// <returns>True if a provider was dequeued, false if the queue is already empty.</returns>
    public bool Next() => _queue.TryDequeue(out _current);

    /// <summary>Clears the queue.</summary>
    public void Clear() => _queue.Clear();

    /// <inheritdoc />
    public void Dispose()
    {
        _current?.Dispose();
        foreach (var layer in _queue) 
            layer.Dispose();
        _queue.Clear();
    }

}
