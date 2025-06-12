using VoiceChat.Networking;

namespace SecretLabNAudio.Core.SendEngines;

/// <summary>Restricts sending <see cref="AudioMessage"/>s to players based on a predicate.</summary>
public class FilteredSendEngine : SendEngine
{

    /// <summary>The condition to match.</summary>
    public Predicate<Player> Filter { get; }

    /// <summary>Creates a new <see cref="FilteredSendEngine"/>.</summary>
    /// <param name="filter">The condition to match for a player to receive the message.</param>
    public FilteredSendEngine(Predicate<Player> filter) => Filter = filter;

    /// <inheritdoc />
    protected internal override bool Broadcast(Player player, AudioMessage message)
    {
        if (!Filter(player))
            return false;
        base.Broadcast(player, message);
        return true;
    }

}
