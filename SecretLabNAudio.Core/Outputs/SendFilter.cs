namespace SecretLabNAudio.Core.Outputs;

public abstract class SendFilter
{

    public abstract void Broadcast<T>(T message) where T : struct, NetworkMessage;

}
