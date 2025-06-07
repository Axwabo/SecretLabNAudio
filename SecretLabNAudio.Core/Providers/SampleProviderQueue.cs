using System.Collections.Generic;
using NAudio.Wave;

namespace SecretLabNAudio.Core.Providers;

public sealed class SampleProviderQueue : ISampleProvider
{

    private readonly Queue<ISampleProvider> _queue = [];

    public IReadOnlyCollection<ISampleProvider> Queue => _queue;

    private ISampleProvider? _current;

    public SampleProviderQueue(WaveFormat waveFormat) => WaveFormat = waveFormat;

    public WaveFormat WaveFormat { get; }

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

    public void Enqueue(ISampleProvider provider)
    {
        if (!WaveFormat.Equals(provider.WaveFormat))
            throw new FormatException("The provider's wave format must match the queue's format.");
        _queue.Enqueue(provider);
    }

    public bool Next() => _queue.TryDequeue(out _current);

    public void ClearQueue() => _queue.Clear();

}
