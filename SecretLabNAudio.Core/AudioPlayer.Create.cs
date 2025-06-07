using LabApi.Features.Wrappers;
using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.Core;

public partial class AudioPlayer
{

    private static AudioPlayer Init(GameObject o, byte id, SpeakerSettings settings)
    {
        var player = o.AddComponent<AudioPlayer>();
        player.Id = id;
        player.ApplySettings(settings);
        NetworkServer.Spawn(o);
        return player;
    }

    public static AudioPlayer Create(byte id, SpeakerSettings settings, Vector3 position = default)
        => Init(SpeakerToy.Create(position, Quaternion.identity, networkSpawn: false).GameObject, id, settings);

    public static AudioPlayer Create(byte id, SpeakerSettings settings, Transform parent, Vector3 position = default)
        => Init(SpeakerToy.Create(position, Quaternion.identity, parent, false).GameObject, id, settings);

}
