using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;

namespace SecretLabNAudio.Core;

public partial class AudioPlayer
{

    /// <inheritdoc cref="SpeakerToyPool.TryGetNextAvailableId" />
    public static bool TryGetNextAvailableId(out byte result) => SpeakerToyPool.TryGetNextAvailableId(out result);

    /// <inheritdoc cref="SpeakerToyPool.NextAvailableId"/>
    public static byte NextAvailableId => SpeakerToyPool.NextAvailableId;

    /// <summary>
    /// Creates a new <see cref="SpeakerToy"/> with an <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="id">The controller ID of the player.</param>
    /// <param name="settings">The settings to apply to the player.</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <returns>A new <see cref="AudioPlayer"/>.</returns>
    public static AudioPlayer Create(byte id, SpeakerSettings settings, Transform? parent = null, Vector3 position = default, bool spawn = true)
    {
        var o = SpeakerToy.Create(position, Quaternion.identity, parent, false).GameObject;
        var player = o.AddComponent<AudioPlayer>()
            .WithId(id)
            .ApplySettings(settings);
        if (spawn)
            NetworkServer.Spawn(o);
        return player;
    }

    /// <summary>
    /// Creates a new <see cref="SpeakerToy"/> with an <see cref="AudioPlayer"/>, and sets its ID to <see cref="NextAvailableId"/>.
    /// </summary>
    /// <param name="settings">The settings to apply to the player.</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <exception cref="OverflowException">Thrown when no IDs are available.</exception>
    /// <returns>A new <see cref="AudioPlayer"/>.</returns>
    public static AudioPlayer Create(SpeakerSettings settings, Vector3 position = default, Transform? parent = null, bool spawn = true)
        => Create(NextAvailableId, settings, parent, position, spawn);

    /// <summary>
    /// Creates a new <see cref="SpeakerToy"/> with <see cref="SpeakerSettings.Default">default settings</see>, and adds an <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="id"></param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <returns>A new <see cref="AudioPlayer"/>.</returns>
    public static AudioPlayer CreateDefault(byte id, Vector3 position = default, Transform? parent = null, bool spawn = true)
        => Create(id, SpeakerSettings.Default, parent, position, spawn);

    /// <summary>
    /// Creates a new <see cref="SpeakerToy"/> with <see cref="SpeakerSettings.Default">default settings</see>, adds an <see cref="AudioPlayer"/>, and sets its ID to <see cref="NextAvailableId"/>.
    /// </summary>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <exception cref="OverflowException">Thrown when no IDs are available.</exception>
    /// <returns>A new <see cref="AudioPlayer"/>.</returns>
    public static AudioPlayer CreateDefault(Vector3 position = default, Transform? parent = null, bool spawn = true)
        => CreateDefault(NextAvailableId, position, parent, spawn);

    /// <summary>
    /// Creates a new <see cref="SpeakerSettings.GloballyAudible">globally audible</see> <see cref="SpeakerToy"/>, and adds an <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="id"></param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <returns>A new <see cref="AudioPlayer"/>.</returns>
    public static AudioPlayer CreateGlobal(byte id, Vector3 position = default, Transform? parent = null, bool spawn = true)
        => Create(id, SpeakerSettings.Default, parent, position, spawn);

    /// <summary>
    /// Creates a new <see cref="SpeakerSettings.GloballyAudible">globally audible</see> <see cref="SpeakerToy"/>, adds an <see cref="AudioPlayer"/>, and sets its ID to <see cref="NextAvailableId"/>.
    /// </summary>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <exception cref="OverflowException">Thrown when no IDs are available.</exception>
    /// <returns>A new <see cref="AudioPlayer"/>.</returns>
    public static AudioPlayer CreateGlobal(Vector3 position = default, Transform? parent = null, bool spawn = true)
        => CreateGlobal(NextAvailableId, position, parent, spawn);

}
