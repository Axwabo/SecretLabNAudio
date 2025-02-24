using CentralAuth;
using VoiceChat.Networking;

namespace SecretLabNAudio.Core.SendEngines;

public class SendEngine
{

    public void Broadcast(AudioMessage message)
    {
        foreach (var hub in ReferenceHub.AllHubs)
            if (hub.Mode == ClientInstanceMode.ReadyClient)
                Broadcast(hub, message);
    }

    protected virtual void Broadcast(ReferenceHub hub, AudioMessage message) => hub.connectionToClient.Send(message);

}
