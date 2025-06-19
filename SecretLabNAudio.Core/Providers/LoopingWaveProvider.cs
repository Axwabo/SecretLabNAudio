namespace SecretLabNAudio.Core.Providers;

/// <summary>Wraps a <see cref="WaveStream"/> and restarts it when reaching the end.</summary>
public sealed class LoopingWaveProvider : IWaveProvider, IDisposable
{

    /// <summary>The <see cref="WaveStream"/> to loop.</summary>
    public WaveStream Stream { get; }

    /// <summary>Creates a new <see cref="LoopingWaveProvider"/>.</summary>
    /// <param name="stream">The <see cref="WaveStream"/> to loop.</param>
    public LoopingWaveProvider(WaveStream stream) => Stream = stream;

    /// <inheritdoc />
    public WaveFormat WaveFormat => Stream.WaveFormat;

    /// <inheritdoc />
    public int Read(byte[] buffer, int offset, int count)
    {
        var read = Stream.Read(buffer, offset, count);
        if (read == count)
            return count;
        Stream.Position = 0;
        read += Stream.Read(buffer, offset + read, count - read);
        return read;
    }

    /// <inheritdoc />
    public void Dispose() => Stream.Dispose();

}
