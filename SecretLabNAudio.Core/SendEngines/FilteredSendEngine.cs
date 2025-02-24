using VoiceChat.Networking;

namespace SecretLabNAudio.Core.SendEngines;

public class FilteredSendEngine : SendEngine
{

    public Predicate<ReferenceHub> Filter { get; }

    public FilteredSendEngine(AudioPlayer player, Predicate<ReferenceHub> filter) : base(player) => Filter = filter;

    protected override void Broadcast(ReferenceHub hub, AudioMessage message)
    {
        if (Filter(hub))
            base.Broadcast(hub, message);
    }

}
