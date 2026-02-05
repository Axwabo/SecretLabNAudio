using NAudio.Utils;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Extensions.Processors;

namespace SecretLabNAudio.Core.Processors;

/// <summary>
/// A delegate that's invoked after a mixer input provided fewer samples than requested.
/// </summary>
/// <param name="input">The input that has ended.</param>
/// <param name="keep">Whether to keep the input in the mixer, allowing for it to provide samples when new data is available.</param>
public delegate void MixerInputEnded(MixerInput input, ref bool keep);

/// <summary>
/// An audio processor that mixes inputs together.
/// </summary>
public sealed class Mixer : IAudioProcessor
{

    [ThreadStatic]
    private static float[]? _readBuffer;

    private readonly List<MixerInput> _inputs = [];

    /// <summary>The list of inputs.</summary>
    public IReadOnlyList<MixerInput> Inputs { get; }

    /// <summary>
    /// Creates a mixer with no inputs.
    /// </summary>
    /// <param name="waveFormat">The format of the mixer.</param>
    public Mixer(WaveFormat waveFormat)
    {
        WaveFormat = waveFormat;
        Inputs = _inputs.AsReadOnly();
    }

    /// <summary>
    /// Creates a mixer with a single anonymous input.
    /// </summary>
    /// <include file='../XmlDocs/Mixer.xml' path='doc/Add/Anonymous/param'/>
    /// <remarks>The <see cref="WaveFormat"/> will be set to the <paramref name="input"/>'s format.</remarks>
    public Mixer(ISampleProvider input, bool isOwned = true) : this(input.WaveFormat)
        => AddAnonymous(input, isOwned);

    /// <summary>
    /// Creates a mixer with a single named input.
    /// </summary>
    /// <include file='../XmlDocs/Mixer.xml' path='doc/Add/Named/param'/>
    /// <remarks>The <see cref="WaveFormat"/> will be set to the <paramref name="input"/>'s format.</remarks>
    public Mixer(ISampleProvider input, string name, bool isOwned = true) : this(input.WaveFormat)
        => AddNamed(input, name, isOwned);

    /// <inheritdoc />
    public WaveFormat WaveFormat { get; }

    /// <include file='../XmlDocs/Providers.xml' path='doc/ReadFully/summary'/>
    public bool ReadFully { get; set; }

    /// <summary>Invoked after an input provided fewer samples than requested.</summary>
    public event MixerInputEnded? InputEnded;

    /// <summary>
    /// Adds a named input to the mixer.
    /// </summary>
    /// <include file='../XmlDocs/Mixer.xml' path='doc/Add/Named/param'/>
    /// <returns>The mixer itself.</returns>
    /// <include file='../XmlDocs/Mixer.xml' path='doc/Add/exception'/>
    public Mixer AddNamed(ISampleProvider input, string name, bool isOwned = true)
    {
        ThrowIfIncompatible(input, isOwned);
        _inputs.Add(new MixerInput(name, input, isOwned));
        return this;
    }

    /// <summary>
    /// Adds an anonymous input to the provider (with a <see langword="null"/> <see cref="MixerInput.Name"/>).
    /// </summary>
    /// <include file='../XmlDocs/Mixer.xml' path='doc/Add/Anonymous/param'/>
    /// <returns>The mixer itself.</returns>
    /// <include file='../XmlDocs/Mixer.xml' path='doc/Add/exception'/>
    public Mixer AddAnonymous(ISampleProvider input, bool isOwned = true)
    {
        ThrowIfIncompatible(input, isOwned);
        _inputs.Add(new MixerInput(null, input, isOwned));
        return this;
    }

    /// <summary>
    /// Removes the given <see cref="MixerInput"/> if it's part of the mixer.
    /// </summary>
    /// <param name="input">The input to remove.</param>
    /// <returns>The mixer itself.</returns>
    public Mixer Remove(MixerInput input)
    {
        if (_inputs.Remove(input))
            input.Dispose();
        return this;
    }

    /// <summary>
    /// Removes the first input whose <see cref="ProcessorLayer.Provider"/> equals <paramref name="provider"/>.
    /// </summary>
    /// <param name="provider">The provider to match.</param>
    /// <returns>The mixer itself.</returns>
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

    /// <include file='../XmlDocs/Mixer.xml' path='doc/RemoveAllByName/summary'/>
    /// <param name="name">The name to match.</param>
    /// <param name="comparison">The string comparison method to use.</param>
    /// <returns>The mixer itself.</returns>
    public Mixer RemoveAllByName(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        for (var i = _inputs.Count - 1; i >= 0; i--)
            if (string.Equals(name, _inputs[i].Name, comparison))
                Remove(i);
        return this;
    }

    /// <summary>
    /// Removes all inputs matching the given predicate.
    /// </summary>
    /// <param name="match">The predicate to use.</param>
    /// <returns>The mixer itself.</returns>
    public Mixer RemoveAll(Func<MixerInput, bool> match)
    {
        for (var i = _inputs.Count - 1; i >= 0; i--)
            if (match(_inputs[i]))
                Remove(i);
        return this;
    }

    /// <summary>
    /// Removes all mixer inputs.
    /// </summary>
    /// <returns>The mixer itself.</returns>
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

    private void ThrowIfIncompatible(ISampleProvider input, bool isOwned)
    {
        if (WaveFormat.Matches(input.WaveFormat))
            return;
        if (isOwned)
            (input as IDisposable)?.Dispose();
        throw new ArgumentException("The input's WaveFormat does not match the format of the Mixer.", nameof(input));
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
