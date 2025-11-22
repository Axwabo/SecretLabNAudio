namespace SecretLabNAudio.Core.Processors;

public class StreamAudioProcessor : SampleProviderWrapper
{

    public WaveStream Stream { get; }

    public bool Loop { get; set; }

    public StreamAudioProcessor(WaveStream stream) : this(stream, stream.ToSampleProvider()) => Stream = stream;

    public StreamAudioProcessor(WaveStream stream, ISampleProvider provider) : base(provider, stream) => Stream = stream;

    protected override int ReadFromProvider(ISampleProvider provider, float[] buffer, int offset, int count)
    {
        if (!Loop)
            return provider.Read(buffer, offset, count);
        var total = 0;
        while (total < count)
        {
            var read = provider.Read(buffer, offset, count);
            if (read == 0)
                Stream.Position = 0;
            total += read;
        }

        return total;
    }

}
