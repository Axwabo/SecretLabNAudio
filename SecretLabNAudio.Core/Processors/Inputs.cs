namespace SecretLabNAudio.Core.Processors;

public record ProcessorLayer(ISampleProvider Provider, bool IsOwned);

public sealed record MixerInput(string Name, ISampleProvider Provider, bool IsOwned) : ProcessorLayer(Provider, IsOwned);
