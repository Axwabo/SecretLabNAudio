namespace SecretLabNAudio.Core.Outputs;

public sealed class GlobalFilter : SendFilter
{

    public override void Broadcast<T>(T message)
    {
        foreach (var player in Player.ReadyList)
            player.Connection.Send(message);
    }

}
