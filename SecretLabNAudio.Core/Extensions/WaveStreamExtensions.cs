using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="WaveStream"/> class.</summary>
public static class WaveStreamExtensions
{

    private const int BufferLength = 4800;
    private static readonly float[] Buffer = new float[BufferLength];

    /// <summary>Wraps the stream in a <see cref="LoopingWaveProvider"/>.</summary>
    /// <param name="stream">The <see cref="WaveStream"/> to wrap.</param>
    /// <returns>The <see cref="LoopingWaveProvider"/> wrapping the stream.</returns>
    public static LoopingWaveProvider Loop(this WaveStream stream) => new(stream);

    /// <summary>
    /// Fully reads the stream in an <see cref="AudioPlayer"/>-compatible format and creates a buffer for the read samples.
    /// </summary>
    /// <param name="stream">The <see cref="WaveStream"/> to read samples from.</param>
    /// <param name="seekToBeginning">Whether to rewind the stream to the start before reading samples.</param>
    /// <returns>A <see cref="RawSourceSampleProvider"/> containing the read samples.</returns>
    /// <seealso cref="WaveProviderExtensions.ToPlayerCompatible"/>
    public static RawSourceSampleProvider ReadPlayerCompatibleSamples(this WaveStream stream, bool seekToBeginning = true)
        => stream.ReadSamples(WaveProviderExtensions.ToPlayerCompatible, seekToBeginning);

    /// <summary>
    /// Fully reads the stream and creates a buffer for the read samples.
    /// </summary>
    /// <param name="stream">The <see cref="WaveStream"/> to read samples from.</param>
    /// <param name="seekToBeginning">Whether to rewind the stream to the start before reading samples.</param>
    /// <returns>A <see cref="RawSourceSampleProvider"/> containing the read samples.</returns>
    public static RawSourceSampleProvider ReadSamples(this WaveStream stream, bool seekToBeginning = true)
        => stream.ReadSamples(WaveExtensionMethods.ToSampleProvider, seekToBeginning);

    /// <summary>
    /// Fully reads the stream and creates a buffer for the read samples using a custom <see cref="WaveStream"/> to <see cref="ISampleProvider"/> conversion function.
    /// </summary>
    /// <param name="stream">The <see cref="WaveStream"/> to read samples from.</param>
    /// <param name="toProvider">The function to convert the <see cref="WaveStream"/> to an <see cref="ISampleProvider"/>.</param>
    /// <param name="seekToBeginning">Whether to rewind the stream to the start before reading samples.</param>
    /// <returns>A <see cref="RawSourceSampleProvider"/> containing the read samples.</returns>
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
