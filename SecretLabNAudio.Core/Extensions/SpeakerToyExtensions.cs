using AdminToys;

namespace SecretLabNAudio.Core.Extensions;

public static class SpeakerToyExtensions
{

    private static SpeakerToy? _prefab;

    public static SpeakerToy Prefab
    {
        get
        {
            if (_prefab != null)
                return _prefab;
            foreach (var value in NetworkClient.prefabs.Values)
                if (value.TryGetComponent(out SpeakerToy toy))
                    return _prefab = toy;
            throw new MissingComponentException("SpeakerToy not found");
        }
    }

}
