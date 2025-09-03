using PlayerRoles.Voice;
using VoiceChat;
using VoiceChat.Networking;

namespace SecretLabNAudio.Core.SendEngines;

/// <summary>A send engine that broadcasts audio through a <see cref="Player"/> as if they were speaking.</summary>
/// <remarks>Distance & voice checks are not performed.</remarks>
public class VoiceMessageSendEngine : SendEngine
{

    /// <summary>The player to broadcast as.</summary>
    public Player Source { get; }

    /// <summary>The voice channel to broadcast through.</summary>
    public VoiceChatChannel Channel { get; set; }

    /// <summary>Whether to call the <see cref="VoiceModuleBase.ValidateReceive"/> method to check if the message should be sent to the recipient.</summary>
    public bool ValidateReceive { get; set; }

    private bool _destroyed;

    /// <summary>Creates a new <see cref="VoiceMessageSendEngine"/>.</summary>
    /// <param name="source">The player to broadcast as.</param>
    /// <param name="channel">The voice channel to broadcast through.</param>
    public VoiceMessageSendEngine(Player source, VoiceChatChannel channel)
    {
        Source = source;
        Channel = channel;
    }

    /// <summary>
    /// Broadcasts the audio message through the <see cref="Source"/> player as a voice message.
    /// </summary>
    /// <inheritdoc/>
    protected internal sealed override bool Broadcast(Player player, AudioMessage message)
    {
        if (_destroyed)
            return false;
        if (!Source.ReferenceHub)
        {
            _destroyed = true;
            return false;
        }

        if (ShouldReceive(player))
            player.Connection.Send(new VoiceMessage(Source.ReferenceHub, Channel, message.Data, message.DataLength, false));
        return true;
    }

    /// <summary>
    /// Checks whether the <see cref="player"/> should receive the voice message.
    /// </summary>
    /// <param name="player">The player to check.</param>
    /// <returns>True if the player should receive the message.</returns>
    protected virtual bool ShouldReceive(Player player)
        => !ValidateReceive
           || player.RoleBase is IVoiceRole {VoiceModule: var module} && module.ValidateReceive(Source.ReferenceHub, Channel) != VoiceChatChannel.None;

}
