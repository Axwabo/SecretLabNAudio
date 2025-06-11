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
    public const int PacketSamples = SampleRate / PacketsPerSecond;

    /// <summary>The duration of a packet in seconds.</summary>
    public const float PacketDuration = 1f / PacketsPerSecond;

    /// <summary>The <see cref="WaveFormat"/> supported by <see cref="AudioPlayer"/> instances.</summary>
    public static WaveFormat SupportedFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels);

}
