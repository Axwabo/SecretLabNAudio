namespace SecretLabNAudio.Core.Pools;

/// <summary>Provides methods to reuse <see cref="AudioPlayer"/> components.</summary>
public static class AudioPlayerPool
{

    /// <inheritdoc cref="SpeakerToyPool.NextAvailableId" />
    public static byte NextAvailableId => SpeakerToyPool.NextAvailableId;

    /// <summary>
    /// Rents an <see cref="AudioPlayer"/> from the pool or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// </summary>
    /// <param name="id">The controller ID to assign to the player.</param>
    /// <param name="settings">The settings to apply to the player.</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    public static AudioPlayer Rent(byte id, SpeakerSettings settings, Transform? parent = null, Vector3 position = default)
    {
        if (!SpeakerToyPool.TryGetFromPool(out var existing, parent, position, false))
            return AudioPlayer.Create(id, settings, parent, position);
        var player = existing.WithId(id)
            .ApplySettings(settings)
            .AddAudioPlayer();
        NetworkServer.Spawn(player.gameObject);
        return player;
    }

    /// <summary>
    /// Rents an <see cref="AudioPlayer"/> with the next available ID from the pool or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// </summary>
    /// <param name="settings">The settings to apply to the player.</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the player to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the speaker in local space (world space if no parent is specified).</param>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    /// <seealso cref="NextAvailableId"/>
    public static AudioPlayer Rent(SpeakerSettings settings, Transform? parent = null, Vector3 position = default)
        => Rent(NextAvailableId, settings, parent, position);

    /// <summary>Returns an <see cref="AudioPlayer"/> to the pool.</summary>
    /// <param name="player">The player to return.</param>
    public static void Return(AudioPlayer player) => SpeakerToyPool.Return(player.Speaker);

}
