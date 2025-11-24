using System.Collections.Generic;

namespace SecretLabNAudio.Core.Processors;

public delegate void MixerInputEnded(MixerInput input);

public sealed class Mixer : IAudioProcessor
{

    private readonly List<MixerInput> _inputs = [];

    public Mixer(IAudioProcessor input) : this(input.WaveFormat)
    {
        WaveFormat = input.WaveFormat;
        AddAnonymous(input);
    }

    public Mixer(WaveFormat waveFormat) => WaveFormat = waveFormat;

    public IReadOnlyList<MixerInput> Inputs => _inputs.AsReadOnly();

    public WaveFormat WaveFormat { get; }

    public bool ReadFully { get; set; }

    public void AddNamed(ISampleProvider input, string name, bool isOwned = true) => _inputs.Add(new MixerInput(name, input, isOwned));

    public void AddAnonymous(ISampleProvider input, bool isOwned = true) => _inputs.Add(new MixerInput(null, input, isOwned));

    public void Remove(ISampleProvider input)
    {
        for (var i = 0; i < _inputs.Count; i++)
        {
            if (_inputs[i].Provider != input)
                continue;
            _inputs.RemoveAt(i);
            break;
        }
    }

    public void RemoveAll(Func<MixerInput, bool> match)
    {
        for (var i = _inputs.Count - 1; i >= 0; i--)
            if (match(_inputs[i]))
                _inputs.RemoveAt(i);
    }

    /// <inheritdoc />
    public int Read(float[] buffer, int offset, int count)
    {
        var read = 0;
        foreach (var (_, provider, _) in Inputs)
            read = Math.Max(read, provider.Read(buffer, offset, count));
        if (ReadFully && read < count)
            Array.Clear(buffer, read, count - read);
        return read;
    }

    /// <inheritdoc />
    public void Dispose() => _inputs.DisposeAllAndClear();

}
