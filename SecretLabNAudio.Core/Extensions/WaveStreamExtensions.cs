using NAudio.Wave;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions;

public static class WaveStreamExtensions
{

    private const int BufferLength = 4800;
    private static readonly float[] Buffer = new float[BufferLength];

    public static LoopingWaveProvider Loop(this WaveStream stream) => new(stream);

    public static RawSourceSampleProvider ReadPlayerCompatibleSamples(this WaveStream stream, bool seekToBeginning = true)
        => stream.ReadSamples(WaveProviderExtensions.ToPlayerCompatible, seekToBeginning);

    public static RawSourceSampleProvider ReadSamples(this WaveStream stream, bool seekToBeginning = true)
        => stream.ReadSamples(WaveExtensionMethods.ToSampleProvider, seekToBeginning);

    public static RawSourceSampleProvider ReadSamples(this WaveStream stream, Func<WaveStream, ISampleProvider> toProvider, bool seekToBeginning = true)
    {
        if (seekToBeginning && stream.CanSeek)
            stream.Position = 0;
        var provider = toProvider(stream);
        var sampleCount = provider.WaveFormat.SampleCount(stream.TotalTime.TotalSeconds);
        var align = stream.BlockAlign * (stream.WaveFormat.BitsPerSample / 8);
        sampleCount += sampleCount % align;
        var array = new float[sampleCount];
        var total = 0;
        int read;
        while ((read = provider.Read(array, total, Math.Min(BufferLength, array.Length - total))) != 0)
            total += read;
        ReadRemaining(provider, ref array, ref total);
        return new RawSourceSampleProvider(array, total, provider.WaveFormat);
    }

    private static void ReadRemaining(ISampleProvider provider, ref float[] array, ref int total)
    {
        var read = provider.Read(Buffer, 0, BufferLength);
        if (read == 0)
            return;
        Array.Resize(ref array, total + read);
        Array.Copy(Buffer, 0, array, total, read);
        total += read;
        while ((read = provider.Read(Buffer, 0, BufferLength)) != 0)
        {
            Array.Resize(ref array, array.Length + BufferLength);
            Array.Copy(Buffer, 0, array, total, read);
            total += read;
        }
    }

}
