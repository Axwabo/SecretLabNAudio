namespace SecretLabNAudio.Core.Processors;

public record ProcessorLayer(ISampleProvider Provider, bool IsOwned = true);

public sealed record MixerInput(string? Name, ISampleProvider Provider, bool IsOwned = true) : ProcessorLayer(Provider, IsOwned);
