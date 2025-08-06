namespace SecretLabNAudio.Core;

public interface IAudioPacketMonitor
{

    void OnRead(float[] buffer, int count);

    void OnEmpty();

}
