namespace SecretLabNAudio.Core.Outputs;

public sealed class SinglePlayerFilter : SendFilter
{

    public Player Player { get; }

    public SinglePlayerFilter(Player player) => Player = player;

    public override void Broadcast<T>(T message) => Player.Connection.Send(message);

}
