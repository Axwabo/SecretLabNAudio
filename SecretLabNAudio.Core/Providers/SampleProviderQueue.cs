using System.Collections.Generic;

namespace SecretLabNAudio.Core.Providers;

/// <summary>A sample provider reading from a queue of providers.</summary>
public sealed class SampleProviderQueue : ISampleProvider
{

    private readonly Queue<ISampleProvider> _queue = [];

    /// <summary>A read-only collection representing the underlying queue.</summary>
    public IReadOnlyCollection<ISampleProvider> Queue => _queue;

    private ISampleProvider? _current;

    /// <summary>Creates a new <see cref="SampleProviderQueue"/>.</summary>
    /// <param name="waveFormat">The <see cref="WaveFormat"/> of the audio.</param>
    public SampleProviderQueue(WaveFormat waveFormat) => WaveFormat = waveFormat;

    /// <inheritdoc/>
    public WaveFormat WaveFormat { get; }

    /// <inheritdoc/>
    public int Read(float[] buffer, int offset, int count)
    {
        if (_current == null && !_queue.TryDequeue(out _current))
            return 0;
        var total = 0;
        while (total < count)
        {
            var target = count - total;
            var read = _current.Read(buffer, total, target);
            total += read;
            if (read < target && !Next())
                break;
        }

        return total;
    }

    /// <summary>Queues a provider to be read from.</summary>
    /// <param name="provider">The <see cref="ISampleProvider"/> to queue.</param>
    /// <exception cref="FormatException">Thrown if the provider's wave format does not match this provider's <see cref="WaveFormat"/>.</exception>
    public void Enqueue(ISampleProvider provider)
    {
        if (!WaveFormat.Equals(provider.WaveFormat))
            throw new FormatException("The provider's wave format must match the queue's format.");
        _queue.Enqueue(provider);
    }

    /// <summary>Dequeues the next provider in the queue.</summary>
    /// <returns>True if a provider was dequeued, false if the queue is already empty.</returns>
    public bool Next() => _queue.TryDequeue(out _current);

    /// <summary>Clears the queue.</summary>
    public void Clear() => _queue.Clear();

}
