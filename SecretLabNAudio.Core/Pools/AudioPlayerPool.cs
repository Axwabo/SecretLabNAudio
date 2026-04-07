using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.Core.Pools;

/// <summary>Provides methods to reuse <see cref="AudioPlayer"/> components.</summary>
public static class AudioPlayerPool
{

    /// <inheritdoc cref="SpeakerToyPool.TryGetNextAvailableId" />
    public static bool TryGetNextAvailableId(out byte result) => SpeakerToyPool.TryGetNextAvailableId(out result);

    /// <inheritdoc cref="SpeakerToyPool.NextAvailableId" />
    public static byte NextAvailableId => SpeakerToyPool.NextAvailableId;

    /// <summary>
    /// Rents an <see cref="AudioPlayer"/> from the pool, or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// </summary>
    /// <param name="id">The controller ID to assign to the player.</param>
    /// <param name="settings">The settings to apply to the player.</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    public static AudioPlayer Rent(byte id, SpeakerSettings settings, Transform? parent = null, Vector3 position = default, bool spawn = true)
    {
        if (!SpeakerToyPool.TryGetFromPool(out var existing, parent, position, false))
            return AudioPlayer.Create(id, settings, parent, position, spawn);
        var player = existing.WithId(id)
            .ApplySettings(settings)
            .AddAudioPlayer();
        if (spawn)
            NetworkServer.Spawn(player.gameObject);
        return player;
    }

    /// <summary>
    /// Rents an <see cref="AudioPlayer"/> with the next available ID from the pool, or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// </summary>
    /// <param name="settings">The settings to apply to the player.</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    /// <exception cref="OverflowException">Thrown when no IDs are available.</exception>
    /// <seealso cref="NextAvailableId"/>
    public static AudioPlayer Rent(SpeakerSettings settings, Transform? parent = null, Vector3 position = default, bool spawn = true)
        => Rent(NextAvailableId, settings, parent, position, spawn);

    /// <summary>
    /// Rents an <see cref="AudioPlayer"/> with the next available ID from the pool, or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// Applies <see cref="SpeakerSettings.Default"/>.
    /// </summary>
    /// <param name="id">The controller ID to assign to the player.</param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    /// <seealso cref="NextAvailableId"/>
    public static AudioPlayer RentDefault(byte id, Vector3 position = default, Transform? parent = null, bool spawn = true)
        => Rent(id, SpeakerSettings.Default, parent, position, spawn);

    /// <summary>
    /// Rents an <see cref="AudioPlayer"/> with the next available ID from the pool, or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// Applies <see cref="SpeakerSettings.Default"/>.
    /// </summary>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    /// <exception cref="OverflowException">Thrown when no IDs are available.</exception>
    /// <seealso cref="NextAvailableId"/>
    public static AudioPlayer RentDefault(Vector3 position = default, Transform? parent = null, bool spawn = true)
        => Rent(NextAvailableId, SpeakerSettings.Default, parent, position, spawn);

    /// <summary>
    /// Rents an <see cref="AudioPlayer"/> with the next available ID from the pool, or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// Applies <see cref="SpeakerSettings.GloballyAudible"/>.
    /// </summary>
    /// <param name="id">The controller ID to assign to the player.</param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    /// <seealso cref="NextAvailableId"/>
    public static AudioPlayer RentGloballyAudible(byte id, Vector3 position = default, Transform? parent = null, bool spawn = true)
        => Rent(id, SpeakerSettings.GloballyAudible, parent, position, spawn);

    /// <summary>
    /// Rents an <see cref="AudioPlayer"/> with the next available ID from the pool, or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// Applies <see cref="SpeakerSettings.GloballyAudible"/>.
    /// </summary>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    /// <exception cref="OverflowException">Thrown when no IDs are available.</exception>
    /// <seealso cref="NextAvailableId"/>
    public static AudioPlayer RentGloballyAudible(Vector3 position = default, Transform? parent = null, bool spawn = true)
        => Rent(NextAvailableId, SpeakerSettings.GloballyAudible, parent, position, spawn);

    /// <summary>Returns an <see cref="AudioPlayer"/> to the pool.</summary>
    /// <param name="player">The player to return.</param>
    public static void Return(AudioPlayer player)
    {
        if (player)
            SpeakerToyPool.Return(player.Speaker);
    }

    /// <summary>Checks whether the given player's speaker is currently pooled.</summary>
    /// <param name="player">The player to check the speaker of.</param>
    /// <returns>Whether the player's speaker is in the pool.</returns>
    public static bool IsPooled(AudioPlayer player) => SpeakerToyPool.IsPooled(player.Speaker);

}
