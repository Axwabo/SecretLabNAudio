namespace SecretLabNAudio.Core.Opus;

public sealed class OpusAudioProcessor : IAudioProcessor
{

    private readonly IOpusPacketProvider _provider;

    public OpusAudioProcessor(IOpusPacketProvider provider)
    {
        _provider = provider;
    }

    public void Dispose() => _provider.Dispose();

    public int Read(float[] buffer, int offset, int count) => throw new NotImplementedException();

    public WaveFormat WaveFormat { get; }

}
