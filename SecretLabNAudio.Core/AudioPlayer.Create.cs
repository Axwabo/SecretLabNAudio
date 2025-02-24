using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.Core;

public partial class AudioPlayer
{

    public static AudioPlayer Create(byte id, AudioPlayerSettings settings, Vector3 position = default)
    {
        var o = Instantiate(SpeakerToyExtensions.Prefab, position, Quaternion.identity).gameObject;
        var player = o.AddComponent<AudioPlayer>();
        player.Id = id;
        player.ApplySettings(settings);
        NetworkServer.Spawn(o);
        return player;
    }

}
