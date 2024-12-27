using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.Core;

public partial class AudioPlayer
{

    public static AudioPlayer Create(byte id, AudioPlayerSettings settings, Vector3 position = default) => Create<AudioPlayer>(id, settings, position);

    public static T Create<T>(byte id, AudioPlayerSettings settings, Vector3 position = default) where T : AudioPlayer
    {
        if (typeof(T).IsAbstract)
            throw new InvalidOperationException("Cannot create an abstract AudioPlayer");
        var o = Instantiate(SpeakerToyExtensions.Prefab, position, Quaternion.identity).gameObject;
        var player = o.AddComponent<T>();
        player.Id = id;
        player.ApplySettings(settings);
        NetworkServer.Spawn(o);
        return player;
    }

}
