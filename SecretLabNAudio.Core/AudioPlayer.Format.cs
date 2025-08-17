namespace SecretLabNAudio.Core;

public partial class AudioPlayer
{

    /// <summary>The sample rate to transmit.</summary>
    public const int SampleRate = 48000;

    /// <summary>The amount of channels to transmit.</summary>
    public const int Channels = 1;

    /// <summary>The amount of packets to be sent every second.</summary>
    public const int PacketsPerSecond = 100;

    /// <summary>The amount of samples in a packet.</summary>
    public const int SamplesPerPacket = SampleRate / PacketsPerSecond;

    /// <summary>The duration of a packet in seconds.</summary>
    public const float PacketDuration = 1f / PacketsPerSecond;

    /// <summary>The <see cref="WaveFormat"/> supported by <see cref="AudioPlayer"/> instances.</summary>
    public static WaveFormat SupportedFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels);

    /// <summary>Checks if the given provider is not compatible with <see cref="AudioPlayer"/>s.</summary>
    /// <param name="provider">The provider to check. Null values are skipped.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the given sample provider is not null and does not match the following criteria:
    /// <para>
    /// Encoding = <see cref="WaveFormatEncoding.IeeeFloat"/><br/>
    /// Sample Rate = <see cref="SampleRate"/><br/>
    /// Channels = <see cref="Channels"/>
    /// </para>
    /// </exception>
    public static void ThrowIfIncompatible(ISampleProvider? provider)
    {
        if (provider is {WaveFormat: not {SampleRate: SampleRate, Channels: Channels, Encoding: WaveFormatEncoding.IeeeFloat}})
            throw new ArgumentException($"Expected a mono provider with a sample rate of 48000Hz and IEEEFloat encoding, got format {provider.WaveFormat}");
    }

}
