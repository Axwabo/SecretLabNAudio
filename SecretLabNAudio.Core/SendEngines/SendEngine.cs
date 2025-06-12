using VoiceChat.Networking;

namespace SecretLabNAudio.Core.SendEngines;

/// <summary>The base class for broadcasting <see cref="AudioMessage"/>s.</summary>
public class SendEngine
{

    /// <summary>The default engine that broadcasts to every player.</summary>
    public static SendEngine DefaultEngine { get; } = new();

    /// <summary>Broadcasts the given <see cref="AudioMessage"/>.</summary>
    /// <param name="message">The <see cref="AudioMessage"/> to broadcast.</param>
    public void Broadcast(AudioMessage message)
    {
        foreach (var player in Player.ReadyList)
            Broadcast(player, message);
    }

    /// <summary>
    /// Broadcasts the given <see cref="AudioMessage"/> to the specified <paramref name="player"/>.
    /// </summary>
    /// <param name="player">The <see cref="Player"/> to send the message to.</param>
    /// <param name="message">The <see cref="AudioMessage"/> to send.</param>
    /// <returns>Whether the message was successfully sent.</returns>
    protected internal virtual bool Broadcast(Player player, AudioMessage message)
    {
        player.Connection.Send(message);
        return true;
    }

}
