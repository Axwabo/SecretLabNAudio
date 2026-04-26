namespace SecretLabNAudio.Core;

public partial class AudioPlayer
{

    /// <inheritdoc cref="AudioConstants.SampleRate"/>
    public const int SampleRate = AudioConstants.SampleRate;

    /// <inheritdoc cref="AudioConstants.Channels"/>
    public const int Channels = AudioConstants.Channels;

    /// <inheritdoc cref="AudioConstants.PacketsPerSecond"/>
    public const int PacketsPerSecond = AudioConstants.PacketsPerSecond;

    /// <inheritdoc cref="AudioConstants.SamplesPerPacket"/>
    public const int SamplesPerPacket = AudioConstants.SamplesPerPacket;

    /// <inheritdoc cref="AudioConstants.PacketDuration"/>
    public const float PacketDuration = AudioConstants.PacketDuration;

    /// <inheritdoc cref="AudioConstants.SupportedFormat"/>
    public static WaveFormat SupportedFormat => AudioConstants.SupportedFormat;

    /// <inheritdoc cref="AudioConstants.ThrowIfIncompatible"/>
    public static void ThrowIfIncompatible(ISampleProvider? provider) => AudioConstants.ThrowIfIncompatible(provider);

}
