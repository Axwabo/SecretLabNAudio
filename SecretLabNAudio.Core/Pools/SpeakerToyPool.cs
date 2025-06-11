using System.Diagnostics.CodeAnalysis;

namespace SecretLabNAudio.Core.Pools;

public static class SpeakerToyPool
{

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

    public static SpeakerToy Rent(Transform? parent = null, Vector3 position = default)
        => TryGetFromPool(out var existing, parent, position)
            ? existing
            : SpeakerToy.Create(position, parent);

    public static void Return(SpeakerToy speaker)
    {
        speaker.Stop();
        var o = speaker.GameObject;
        o.SetActive(false);
        o.AddComponent<PooledSpeaker>();
        NetworkServer.DestroyObject(o, NetworkServer.DestroyMode.Reset);
    }

}
