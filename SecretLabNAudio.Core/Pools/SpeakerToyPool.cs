using System.Diagnostics.CodeAnalysis;
using VoiceChat.Playbacks;

namespace SecretLabNAudio.Core.Pools;

/// <summary>Provides methods to reuse <see cref="SpeakerToy"/>s.</summary>
public static class SpeakerToyPool
{

    private static readonly bool[] Occupied = new bool[byte.MaxValue + 1];

    /// <summary>Attempts to get first controller ID not used by any active speakers.</summary>
    /// <param name="result">The first free controller ID. 0 if no ID was found.</param>
    /// <returns>Whether there was an available ID.</returns>
    /// <remarks>
    /// This method only returns false in the scenario when all controller IDs from 0 to 255 are in use.
    /// If all IDs are used, consider <see cref="Return">returning your speakers to the pool</see>
    /// or using the same ID across speakers that have the same output.
    /// </remarks>
    public static bool TryGetNextAvailableId(out byte result)
    {
        Array.Clear(Occupied, 0, Occupied.Length);
        foreach (var instance in SpeakerToyPlaybackBase.AllInstances)
            Occupied[instance.ControllerId] = true;
        for (var i = 0; i < Occupied.Length; i++)
            if (!Occupied[i])
            {
                result = (byte) i;
                return true;
            }

        result = 0;
        return false;
    }

    /// <summary>Gets the first controller ID not used by any active speakers.</summary>
    /// <exception cref="OverflowException">Thrown when no IDs are available.</exception>
    public static byte NextAvailableId => TryGetNextAvailableId(out var id) ? id : throw new OverflowException("No available IDs found");

    /// <summary>Attempts to get a <see cref="SpeakerToy"/> from the pool.</summary>
    /// <param name="toy">The <see cref="SpeakerToy"/> found in the pool, or <see langword="null"/> if none was found.</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the toy to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the toy in local space (world space if no parent is specified).</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <returns>Whether a <see cref="SpeakerToy"/> was found in the pool.</returns>
    /// <remarks>
    /// The <see cref="SpeakerToy.ControllerId"/> of the returned speaker may already be occupied.
    /// Assign an ID yourself by setting it or calling <see cref="SpeakerToyExtensions.WithId"/>.
    /// Speaker settings may also vary.
    /// </remarks>
    public static bool TryGetFromPool([NotNullWhen(true)] out SpeakerToy? toy, Transform? parent = null, Vector3 position = default, bool spawn = true)
    {
        foreach (var existing in PooledSpeaker.Instances)
        {
            var o = existing.gameObject;
            o.SetActive(true);
            toy = existing.Speaker;
            toy.Parent = parent;
            toy.Position = position;
            Object.Destroy(existing);
            if (spawn)
                NetworkServer.Spawn(o);
            return true;
        }

        toy = null;
        return false;
    }

    /// <summary>
    /// Rents a <see cref="SpeakerToy"/> from the pool or creates a new one if no <see cref="SpeakerToy"/> is pooled.
    /// </summary>
    /// <inheritdoc cref="TryGetFromPool" path="remarks"/>
    /// <returns>A new or reused <see cref="SpeakerToy"/>.</returns>
    public static SpeakerToy Rent(Transform? parent = null, Vector3 position = default, bool spawn = true)
        => TryGetFromPool(out var existing, parent, position, spawn)
            ? existing
            : SpeakerToy.Create(position, parent, spawn);

    /// <summary><inheritdoc cref="Rent(Transform?,Vector3,bool)" path="summary"/></summary>
    /// <param name="id">The controller ID to assign to the toy.</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the toy to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the toy in local space (world space if no parent is specified).</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <returns><inheritdoc cref="Rent(Transform?,Vector3,bool)" path="returns"/></returns>
    public static SpeakerToy Rent(byte id, Transform? parent = null, Vector3 position = default, bool spawn = true)
    {
        if (!TryGetFromPool(out var toy, parent, position, false))
            toy = SpeakerToy.Create(position, parent, false);
        toy.ControllerId = id;
        if (spawn)
            toy.Spawn();
        return toy;
    }

    /// <summary><inheritdoc cref="Rent(Transform?,Vector3,bool)" path="summary"/></summary>
    /// <param name="id">The controller ID to assign to the toy.</param>
    /// <param name="settings">Settings to apply to the toy.</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the toy to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the toy in local space (world space if no parent is specified).</param>
    /// <param name="spawn">Whether to invoke <see cref="NetworkServer.Spawn(GameObject,NetworkConnection)"/>.</param>
    /// <returns><inheritdoc cref="Rent(Transform?,Vector3,bool)" path="returns"/></returns>
    public static SpeakerToy Rent(byte id, SpeakerSettings settings, Transform? parent = null, Vector3 position = default, bool spawn = true)
    {
        if (!TryGetFromPool(out var toy, parent, position, false))
            toy = SpeakerToy.Create(position, parent, false);
        toy.WithId(id).ApplySettings(settings);
        if (spawn)
            toy.Spawn();
        return toy;
    }

    /// <summary>Returns a <see cref="SpeakerToy"/> to the pool.</summary>
    /// <param name="speaker">The speaker to return.</param>
    public static void Return(SpeakerToy speaker)
    {
        speaker.Stop();
        if (speaker.IsDestroyed)
            return;
        var o = speaker.GameObject;
        o.SetActive(false);
        if (o.TryGetComponent(out PooledSpeaker _))
            return;
        o.AddComponent<PooledSpeaker>();
        NetworkServer.UnSpawn(o);
    }

}
