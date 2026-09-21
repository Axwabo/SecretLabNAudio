using VoiceChat;

namespace SecretLabNAudio.Core.Outputs;

public class PlayerVoiceOutput : AudioPacketOutput
{

    public Player Player { get; }

    public VoiceChatChannel Channel { get; set; }

    public PlayerVoiceOutput(Player player, VoiceChatChannel channel = VoiceChatChannel.RoundSummary)
    {
        Player = player;
        Channel = channel;
    }

    public override void BroadcastEncodedData(byte[] buffer, int length, SendFilter filter)
        => filter.Broadcast(new VoiceMessage(Player.ReferenceHub, Channel, buffer, length, false));

}
