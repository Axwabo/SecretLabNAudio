using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Outputs;
using VoiceChat;

namespace SecretLabNAudio.Core.Builders;

public readonly struct VoiceAudioPlayerBuilder : IAudioPlayerBuilder, IVoiceBuilder
{

    public AudioPlayer Player { get; }

    public PlayerVoiceOutput Output { get; }

    public static VoiceAudioPlayerBuilder Create(Player source, VoiceChatChannel channel = VoiceChatChannel.RoundSummary)
    {
        var output = new PlayerVoiceOutput(source, channel);
        var player = source.GameObject!.AddComponent<AudioPlayer>().WithPacketOutput(output);
        return new VoiceAudioPlayerBuilder(player, output);
    }

    private VoiceAudioPlayerBuilder(AudioPlayer player, PlayerVoiceOutput output)
    {
        Player = player;
        Output = output;
    }

}
