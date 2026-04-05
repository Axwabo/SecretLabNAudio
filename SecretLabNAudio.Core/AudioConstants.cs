using VoiceChat;

namespace SecretLabNAudio.Core;

/// <summary>
/// Constants and helpers for voice chat audio.
/// </summary>
public static class AudioConstants
{

    /// <summary>The sample rate to transmit.</summary>
    public const int SampleRate = VoiceChatSettings.SampleRate;

    /// <summary>The amount of channels to transmit.</summary>
    public const int Channels = VoiceChatSettings.Channels;

    /// <summary>The amount of packets to be sent every second.</summary>
    public const int PacketsPerSecond = SampleRate * Channels / VoiceChatSettings.PacketSizePerChannel;

    /// <summary>The amount of samples in a packet.</summary>
    public const int SamplesPerPacket = Channels * VoiceChatSettings.PacketSizePerChannel;

    /// <summary>The duration of a packet in seconds.</summary>
    public const float PacketDuration = 1f / PacketsPerSecond;

    /// <summary>The <see cref="WaveFormat"/> supported by <see cref="AudioPlayer"/> instances.</summary>
    public static WaveFormat SupportedFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, Channels);

    /// <summary>Checks if the given provider is not compatible with <see cref="AudioPlayer"/>s.</summary>
    /// <param name="provider">The provider to check. Null values are skipped.</param>
    /// <include file='XmlDocs/Providers.xml' path='doc/Format/exception'/>
    public static void ThrowIfIncompatible(ISampleProvider? provider)
    {
        if (provider is {WaveFormat: not {SampleRate: AudioPlayer.SampleRate, Channels: AudioPlayer.Channels, Encoding: WaveFormatEncoding.IeeeFloat}})
            throw new ArgumentException($"Expected a mono provider with a sample rate of 48000Hz and IEEEFloat encoding, got format {provider.WaveFormat}");
    }

}
