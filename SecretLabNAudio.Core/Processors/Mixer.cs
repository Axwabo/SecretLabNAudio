using System.Collections.Generic;
using SecretLabNAudio.Core.Extensions.Processors;

namespace SecretLabNAudio.Core.Processors;

public delegate void MixerInputEnded(MixerInput input, ref bool keep);

public sealed class Mixer : IAudioProcessor
{

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

    public void AddNamed(ISampleProvider input, string name, bool isOwned = true) => _inputs.Add(new MixerInput(name, input, isOwned));

    public void AddAnonymous(ISampleProvider input, bool isOwned = true) => _inputs.Add(new MixerInput(null, input, isOwned));

    public void Remove(MixerInput input)
    {
        if (_inputs.Remove(input))
            input.Dispose();
    }

    public void Remove(ISampleProvider provider)
    {
        for (var i = 0; i < _inputs.Count; i++)
        {
            if (_inputs[i].Provider != provider)
                continue;
            Remove(i);
            break;
        }
    }

    public void RemoveAllByName(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        for (var i = _inputs.Count - 1; i >= 0; i--)
            if (string.Equals(name, _inputs[i].Name, comparison))
                Remove(i);
    }

    public void RemoveAll(Func<MixerInput, bool> match)
    {
        for (var i = _inputs.Count - 1; i >= 0; i--)
            if (match(_inputs[i]))
                Remove(i);
    }

    public void RemoveAll() => _inputs.DisposeAllAndClear();

    private void Remove(int index)
    {
        _inputs[index].Dispose();
        _inputs.RemoveAt(index);
    }

    /// <inheritdoc />
    public int Read(float[] buffer, int offset, int count)
    {
        var read = 0;
        for (var i = _inputs.Count - 1; i >= 0; i--)
        {
            var input = _inputs[i];
            var provider = input.Provider;
            var readFromProvider = provider.Read(buffer, offset, count);
            if (readFromProvider != 0)
            {
                read = Math.Max(read, readFromProvider);
                continue;
            }

            var keep = false;
            InputEnded?.Invoke(input, ref keep);
            if (keep)
                continue;
            input.Dispose();
            _inputs.RemoveAt(i);
        }

        if (ReadFully && read < count)
            Array.Clear(buffer, read, count - read);
        return read;
    }

    /// <inheritdoc />
    public void Dispose() => RemoveAll();

}
