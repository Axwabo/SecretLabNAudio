using VoiceChat;
using VoiceChat.Networking;

namespace SecretLabNAudio.Core.SendEngines;

/// <summary>A send engine that broadcasts audio through a <see cref="Player"/> as if they were speaking.</summary>
/// <remarks>Distance &amp; voice receiving checks are not performed.</remarks>
public class VoiceMessageSendEngine : SendEngine
{

    /// <summary>The player to broadcast as.</summary>
    public Player Source { get; }

    /// <summary>The voice channel to broadcast through.</summary>
    public VoiceChatChannel Channel { get; set; }

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
    protected internal override bool Broadcast(Player player, AudioMessage message)
    {
        if (_destroyed)
            return false;
        if (!Source.ReferenceHub)
        {
            _destroyed = true;
            return false;
        }

        player.Connection.Send(new VoiceMessage(Source.ReferenceHub, Channel, message.Data, message.DataLength, false));
        return true;
    }

}
