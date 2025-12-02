using NAudio.Utils;
using SecretLabNAudio.Core.Extensions.Processors;

namespace SecretLabNAudio.Core.Processors;

public delegate void MixerInputEnded(MixerInput input, ref bool keep);

public sealed class Mixer : IAudioProcessor
{

    [ThreadStatic]
    private static float[]? _readBuffer;

    private readonly List<MixerInput> _inputs = [];

    public IReadOnlyList<MixerInput> Inputs { get; }

    public Mixer(WaveFormat waveFormat)
    {
        WaveFormat = waveFormat;
        Inputs = _inputs.AsReadOnly();
    }

    public Mixer(ISampleProvider input, bool isOwned = true) : this(input.WaveFormat)
        => AddAnonymous(input, isOwned);

    public Mixer(ISampleProvider input, string name, bool isOwned = true) : this(input.WaveFormat)
        => AddNamed(input, name, isOwned);

    public WaveFormat WaveFormat { get; }

    public bool ReadFully { get; set; }

    public event MixerInputEnded? InputEnded;

    public Mixer AddNamed(ISampleProvider input, string name, bool isOwned = true)
    {
        _inputs.Add(new MixerInput(name, input, isOwned));
        return this;
    }

    public Mixer AddAnonymous(ISampleProvider input, bool isOwned = true)
    {
        _inputs.Add(new MixerInput(null, input, isOwned));
        return this;
    }

    public Mixer Remove(MixerInput input)
    {
        if (_inputs.Remove(input))
            input.Dispose();
        return this;
    }

    public Mixer Remove(ISampleProvider provider)
    {
        for (var i = 0; i < _inputs.Count; i++)
        {
            if (_inputs[i].Provider != provider)
                continue;
            Remove(i);
            break;
        }

        return this;
    }

    public Mixer RemoveAllByName(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        for (var i = _inputs.Count - 1; i >= 0; i--)
            if (string.Equals(name, _inputs[i].Name, comparison))
                Remove(i);
        return this;
    }

    public Mixer RemoveAll(Func<MixerInput, bool> match)
    {
        for (var i = _inputs.Count - 1; i >= 0; i--)
            if (match(_inputs[i]))
                Remove(i);
        return this;
    }

    public Mixer RemoveAll()
    {
        _inputs.DisposeAllAndClear();
        return this;
    }

    private void Remove(int index)
    {
        _inputs[index].Dispose();
        _inputs.RemoveAt(index);
    }

    /// <inheritdoc />
    public int Read(float[] buffer, int offset, int count)
    {
        _readBuffer = BufferHelpers.Ensure(_readBuffer, count);
        var readSpan = _readBuffer.AsSpan()[..count];
        var targetSpan = buffer.AsSpan()[offset..(offset + count)];
        targetSpan.Clear();
        var total = 0;
        for (var i = _inputs.Count - 1; i >= 0; i--)
        {
            var input = _inputs[i];
            var provider = input.Provider;
            var read = provider.Read(_readBuffer, 0, count);
            for (var j = 0; j < read; j++)
                targetSpan[j] += readSpan[j];
            var remove = read < total;
            total = Math.Max(total, read);
            if (!remove)
                continue;
            InputEnded?.Invoke(input, ref remove);
            if (remove)
                Remove(input);
        }

        return ReadFully ? count : total;
    }

    /// <inheritdoc />
    public void Dispose() => RemoveAll();

}
