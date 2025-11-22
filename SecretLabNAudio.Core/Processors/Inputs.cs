namespace SecretLabNAudio.Core.Processors;

public record ProcessorInput(ISampleProvider Provider, bool IsOwned);

public sealed record MixerInput(string Name, ISampleProvider Provider, bool IsOwned) : ProcessorInput(Provider, IsOwned);
