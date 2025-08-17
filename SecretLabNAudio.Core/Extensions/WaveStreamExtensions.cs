using System.Buffers;
using System.Collections.Generic;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="WaveStream"/> class.</summary>
public static class WaveStreamExtensions
{

    private const int BufferLength = 4800;

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
        var totalRead = 0;
        var firstBuffer = ArrayPool<float>.Shared.Rent(BufferLength);
        try
        {
            var read = provider.Read(firstBuffer, 0, BufferLength);
            if (read == 0)
            {
                ArrayPool<float>.Shared.Return(firstBuffer);
                return;
            }

            if (read < BufferLength)
            {
                Array.Resize(ref array, total + read);
                Array.Copy(firstBuffer, 0, array, total, read);
                total += read;
                ArrayPool<float>.Shared.Return(firstBuffer);
                return;
            }

            totalRead += read;
        }
        catch
        {
            ArrayPool<float>.Shared.Return(firstBuffer);
            throw;
        }

        var buffers = new List<float[]> {firstBuffer};
        try
        {
            while (true)
            {
                var buffer = ArrayPool<float>.Shared.Rent(BufferLength);
                var read = provider.Read(buffer, 0, buffer.Length);
                totalRead += read;
                if (read != 0)
                {
                    buffers.Add(buffer);
                    continue;
                }

                ArrayPool<float>.Shared.Return(buffer);
                break;
            }

            Array.Resize(ref array, total + totalRead);
            var copied = 0;
            foreach (var buffer in buffers)
            {
                Array.Copy(buffer, 0, array, total + copied, Math.Min(buffer.Length, totalRead - copied));
                copied += buffer.Length;
            }

            total += totalRead;
        }
        finally
        {
            foreach (var buffer in buffers)
                ArrayPool<float>.Shared.Return(buffer);
        }
    }

}
