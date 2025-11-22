namespace SecretLabNAudio.Core.Processors;

public class StreamAudioProcessor : SampleProviderWrapper
{

    public WaveStream Stream { get; }

    public StreamAudioProcessor(WaveStream stream) : this(stream, stream.ToSampleProvider()) => Stream = stream;

    public StreamAudioProcessor(WaveStream stream, ISampleProvider provider) : base(provider, stream) => Stream = stream;

}
