using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.Core.Groups;

internal sealed class GroupedSpeaker : MonoBehaviour
{

    public static readonly HashSet<GroupedSpeaker> Instances = [];

    public SpeakerToy Speaker { get; private set; } = null!;

    public SpeakerToyGroup Group { get; set; } = null!;

    private void Awake() => Speaker = this.GetSpeaker("GroupedSpeaker must be attached to a SpeakerToy.");

    private void OnEnable() => Instances.Add(this);

    private void OnDisable()
    {
        if (!destroyCancellationToken.IsCancellationRequested)
            Destroy(this);
    }

    private void OnDestroy()
    {
        Instances.Remove(this);
        if (Group.IsDestroyed)
            return;
        if (Speaker == Group.Controller)
            Group.Destroy();
        else
            Group.Remove(Speaker);
    }

}
