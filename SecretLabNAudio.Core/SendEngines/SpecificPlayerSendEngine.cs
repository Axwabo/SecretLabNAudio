using VoiceChat.Networking;

namespace SecretLabNAudio.Core.SendEngines;

/// <summary>A send engine that targets a specific player.</summary>
/// <remarks>
/// Consider your use case before using this class.
/// If you want to send the same output to potentially multiple players, use the <see cref="FilteredSendEngine"/>.
/// </remarks>
public sealed class SpecificPlayerSendEngine : SendEngine
{

    /// <summary>The player to send <see cref="AudioMessage"/>s to.</summary>
    public Player Target { get; }

    /// <summary>Creates a new <see cref="SpecificPlayerSendEngine"/>.</summary>
    /// <param name="target">The player to send messages to.</param>
    /// <remarks><inheritdoc cref="SpecificPlayerSendEngine" path="remarks"/></remarks>
    public SpecificPlayerSendEngine(Player target) => Target = target;

    /// <summary>Sends the message to the <see cref="Target"/>.</summary>
    /// <inheritdoc/>
    public override void Broadcast(AudioMessage message) => Broadcast(Target, message);

    /// <inheritdoc/>
    protected internal override bool Broadcast(Player player, AudioMessage message)
        => player.ReferenceHub && base.Broadcast(player, message);

}
