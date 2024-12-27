namespace SecretLabNAudio.Core;

public class TargetedAudioPlayer : AudioPlayer
{

    public Predicate<ReferenceHub>? HubFilter { get; set; }

    protected override bool ShouldSendTo(ReferenceHub hub) => HubFilter == null || HubFilter(hub);

}
