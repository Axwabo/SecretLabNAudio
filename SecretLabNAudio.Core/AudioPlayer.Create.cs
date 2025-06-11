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

    /// <summary>
    /// Creates a new <see cref="SpeakerToy"/> with an <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="id">The controller ID of the player.</param>
    /// <param name="settings">The settings to apply to the player.</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <returns>A new <see cref="AudioPlayer"/>.</returns>
    public static AudioPlayer Create(byte id, SpeakerSettings settings, Transform? parent = null, Vector3 position = default)
        => Init(SpeakerToy.Create(position, Quaternion.identity, parent, false).GameObject, id, settings);

}
