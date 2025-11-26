namespace SecretLabNAudio.Core.Processors;

/// <summary>
/// An extension of the <see cref="ISampleProvider"/> interface, ensuring automatic disposal of encapsulated resources.
/// </summary>
public interface IAudioProcessor : ISampleProvider, IDisposable;
