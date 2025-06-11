namespace SecretLabNAudio.Core;

public partial class AudioPlayer
{

    private static AudioPlayer Init(GameObject o, byte id, SpeakerSettings settings)
    {
        var player = o.AddComponent<AudioPlayer>()
            .WithId(id)
            .ApplySettings(settings);
        NetworkServer.Spawn(o);
        return player;
    }

    public static AudioPlayer Create(byte id, SpeakerSettings settings, Transform? parent = null, Vector3 position = default)
        => Init(SpeakerToy.Create(position, Quaternion.identity, parent, false).GameObject, id, settings);

}
