namespace SecretLabNAudio.Core.Processors;

/// <summary>
/// An extension of the <see cref="ISampleProvider"/> interface, ensuring a disposal method.
/// </summary>
public interface IAudioProcessor : ISampleProvider, IDisposable;
