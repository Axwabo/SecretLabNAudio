namespace SecretLabNAudio.Core.Outputs;

public abstract class AudioPacketOutput
{

    public abstract void BroadcastEncodedData(byte[] buffer, int length, SendFilter filter);

}
