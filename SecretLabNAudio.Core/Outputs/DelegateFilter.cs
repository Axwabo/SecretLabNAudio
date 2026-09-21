namespace SecretLabNAudio.Core.Outputs;

public sealed class DelegateFilter : SendFilter
{

    private readonly Func<Player, bool> _filter;

    public DelegateFilter(Func<Player, bool> filter) => _filter = filter;

    public override void Broadcast<T>(T message)
    {
        foreach (var player in Player.ReadyList)
            if (_filter(player))
                player.Connection.Send(message);
    }

}
