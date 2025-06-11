namespace SecretLabNAudio.Core;

public partial class AudioPlayer
{

    public const int SampleRate = 48000;

    public const int Channels = 1;

    public const int PacketsPerSecond = 100;

    public const int PacketSamples = SampleRate / PacketsPerSecond;

    public const float PacketDuration = 1f / PacketsPerSecond;

    public static WaveFormat SupportedFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels);

}
