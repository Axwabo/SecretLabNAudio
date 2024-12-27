using NAudio.Wave;

namespace SecretLabNAudio.Core;

public partial class AudioPlayer
{

    public const int SampleRate = 48000;

    public const int Channels = 1;

    public static WaveFormat SupportedFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels);

}
