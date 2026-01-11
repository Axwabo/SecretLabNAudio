using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.Core.Groups;

internal class GroupedSpeaker : MonoBehaviour
{

    public static readonly HashSet<GroupedSpeaker> Instances = [];

    public SpeakerGroupMaster Master { get; set; } = null!;

    public SpeakerToy Speaker { get; private set; } = null!;

    protected virtual void Awake() => Speaker = this.GetSpeaker("GroupedSpeaker must be attached to a SpeakerToy.");

    private void OnEnable() => Instances.Add(this);

    private void OnDisable()
    {
        Instances.Remove(this);
        if (!destroyCancellationToken.IsCancellationRequested)
            Destroy(this);
    }

}
