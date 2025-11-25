using System.Collections.Generic;
using NAudio.Utils;
using SecretLabNAudio.Core.Extensions.Processors;

namespace SecretLabNAudio.Core.Processors;

public delegate void MixerInputEnded(MixerInput input, ref bool keep);

public sealed class Mixer : IAudioProcessor
{

    [ThreadStatic]
    private static float[]? _mixerBuffer;

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
        _mixerBuffer = BufferHelpers.Ensure(_mixerBuffer, count);
        var total = 0;
        for (var i = _inputs.Count - 1; i >= 0; i--)
        {
            var input = _inputs[i];
            var provider = input.Provider;
            var read = provider.Read(_mixerBuffer, offset, count);
            MixInto(buffer, offset, read, total);
            var ended = read < total;
            total = Math.Max(total, read);
            if (!ended)
                continue;
            InputEnded?.Invoke(input, ref ended);
            if (ended)
                Remove(input);
        }

        if (ReadFully && total < count)
            Array.Clear(buffer, total, count - total);
        return total;
    }

    private static void MixInto(float[] buffer, int offset, int read, int total)
    {
        for (var j = 0; j < read; j++)
            if (j <= total)
                buffer[offset + j] += _mixerBuffer![j];
            else
                buffer[offset + j] = _mixerBuffer![j];
    }

    /// <inheritdoc />
    public void Dispose() => RemoveAll();

}
