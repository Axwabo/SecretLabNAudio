using System.Collections.Generic;
using LabApi.Features.Wrappers;
using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.Core.Pools;

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
