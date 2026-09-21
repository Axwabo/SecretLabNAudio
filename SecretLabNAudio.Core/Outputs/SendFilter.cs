namespace SecretLabNAudio.Core.Outputs;

public abstract class SendFilter
{

    public static GlobalFilter Default { get; } = new();

    public abstract void Broadcast<T>(T message) where T : struct, NetworkMessage;

}
