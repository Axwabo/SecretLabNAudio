using VoiceChat.Networking;

namespace SecretLabNAudio.Core.SendEngines;

public class FilteredSendEngine : SendEngine
{

    public Predicate<ReferenceHub> Filter { get; }

    public FilteredSendEngine(Predicate<ReferenceHub> filter) => Filter = filter;

    protected internal override bool Broadcast(ReferenceHub hub, AudioMessage message)
    {
        if (!Filter(hub))
            return false;
        base.Broadcast(hub, message);
        return true;
    }

}
