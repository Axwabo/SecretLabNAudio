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
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param"/>
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
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='settings']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='parent']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='position']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='spawn']"/>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/exception"/>
    /// <seealso cref="NextAvailableId"/>
    public static AudioPlayer Rent(SpeakerSettings settings, Transform? parent = null, Vector3 position = default, bool spawn = true)
        => Rent(NextAvailableId, settings, parent, position, spawn);

    /// <summary>
    /// Rents an <see cref="AudioPlayer"/> with the next available ID from the pool, or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// Applies <see cref="SpeakerSettings.Default"/>.
    /// </summary>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='id']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='position']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='parent']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='spawn']"/>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    /// <seealso cref="NextAvailableId"/>
    public static AudioPlayer RentDefault(byte id, Vector3 position = default, Transform? parent = null, bool spawn = true)
        => Rent(id, SpeakerSettings.Default, parent, position, spawn);

    /// <summary>
    /// Rents an <see cref="AudioPlayer"/> with the next available ID from the pool, or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// Applies <see cref="SpeakerSettings.Default"/>.
    /// </summary>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='position']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='parent']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='spawn']"/>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/exception"/>
    /// <seealso cref="NextAvailableId"/>
    public static AudioPlayer RentDefault(Vector3 position = default, Transform? parent = null, bool spawn = true)
        => Rent(NextAvailableId, SpeakerSettings.Default, parent, position, spawn);

    /// <summary>
    /// Rents an <see cref="AudioPlayer"/> with the next available ID from the pool, or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// Applies <see cref="SpeakerSettings.GloballyAudible"/>.
    /// </summary>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='id']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='position']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='parent']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='spawn']"/>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    /// <seealso cref="NextAvailableId"/>
    public static AudioPlayer RentGloballyAudible(byte id, Vector3 position = default, Transform? parent = null, bool spawn = true)
        => Rent(id, SpeakerSettings.GloballyAudible, parent, position, spawn);

    /// <summary>
    /// Rents an <see cref="AudioPlayer"/> with the next available ID from the pool, or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// Applies <see cref="SpeakerSettings.GloballyAudible"/>.
    /// </summary>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='position']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='parent']"/>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/param[@name='spawn']"/>
    /// <returns>A new or reused <see cref="AudioPlayer"/>.</returns>
    /// <include file="../XmlDocs/Pools.xml" path="doc/Player/exception"/>
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
