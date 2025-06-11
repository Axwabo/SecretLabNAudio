using System.Diagnostics.CodeAnalysis;

namespace SecretLabNAudio.Core.Pools;

/// <summary>Provides methods to reuse <see cref="SpeakerToy"/>s.</summary>
public static class SpeakerToyPool
{

    /// <summary>
    /// Attempts to get a <see cref="SpeakerToy"/> from the pool.
    /// </summary>
    /// <param name="toy">The <see cref="SpeakerToy"/> found in the pool, or <see langword="null"/> if none was found.</param>
    /// <param name="parent">The <see cref="Transform"/> to parent the toy to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position">The position of the toy in local space (world space if no parent is specified).</param>
    /// <param name="spawn">Whether to spawn the toy on the network after retrieving it.</param>
    /// <returns>Whether a <see cref="SpeakerToy"/> was found in the pool.</returns>
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
    /// <param name="parent">The <see cref="Transform"/> to parent the toy to. <see langword="null"/> if it should not be parented.</param>
    /// <param name="position"> The position of the toy in local space (world space if no parent is specified).</param>
    /// <returns>A new or reused <see cref="SpeakerToy"/>.</returns>
    public static SpeakerToy Rent(Transform? parent = null, Vector3 position = default)
        => TryGetFromPool(out var existing, parent, position)
            ? existing
            : SpeakerToy.Create(position, parent);

    /// <summary>Returns a <see cref="SpeakerToy"/> to the pool.</summary>
    /// <param name="speaker">The speaker to return.</param>
    public static void Return(SpeakerToy speaker)
    {
        speaker.Stop();
        if (speaker.IsDestroyed)
            return;
        var o = speaker.GameObject;
        o.SetActive(false);
        o.AddComponent<PooledSpeaker>();
        NetworkServer.DestroyObject(o, NetworkServer.DestroyMode.Reset);
    }

}
