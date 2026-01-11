namespace SecretLabNAudio.Core.Groups;

internal sealed class SpeakerGroupMaster : GroupedSpeaker
{

    public HashSet<GroupedSpeaker> Children { get; } = [];

    protected override void Awake()
    {
        base.Awake();
        Master = this;
    }

}
