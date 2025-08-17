using System.Collections.Generic;

namespace SecretLabNAudio.Core.Pools;

[DisallowMultipleComponent]
internal sealed class PooledSpeaker : MonoBehaviour
{

    public static readonly HashSet<PooledSpeaker> Instances = [];

    public SpeakerToy Speaker { get; private set; } = null!;

    private void Awake()
    {
        Speaker = this.GetSpeaker("PooledSpeaker must be attached to a SpeakerToy.");
        Instances.Add(this);
    }

    private void OnDestroy() => Instances.Remove(this);

}
