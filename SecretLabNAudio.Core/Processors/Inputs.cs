namespace SecretLabNAudio.Core.Processors;

/// <summary>
/// Ownership definition object for <see cref="IAudioProcessor"/>s. 
/// </summary>
/// <param name="Provider">The <see cref="ISampleProvider"/> to read from.</param>
/// <param name="IsOwned">Whether to dispose of the <paramref name="Provider"/> when the encapsulating object is disposed.</param>
public record ProcessorLayer(ISampleProvider Provider, bool IsOwned = true);

/// <summary>
/// A named ownership definition object for <see cref="Mixer"/>s.
/// </summary>
/// <param name="Name">The name of the mixer input. May be null (anonymous).</param>
/// <param name="Provider">The <see cref="ISampleProvider"/> to read from.</param>
/// <param name="IsOwned">Whether to dispose of the <paramref name="Provider"/> when the encapsulating object is disposed.</param>
public sealed record MixerInput(string? Name, ISampleProvider Provider, bool IsOwned = true) : ProcessorLayer(Provider, IsOwned);
