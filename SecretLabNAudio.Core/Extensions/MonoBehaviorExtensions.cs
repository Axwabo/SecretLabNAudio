using LabApi.Features.Wrappers;

namespace SecretLabNAudio.Core.Extensions;

public static class MonoBehaviorExtensions
{

    public static SpeakerToy GetSpeaker(this MonoBehaviour behavior, string exceptionMessage)
    {
        if (!behavior.TryGetComponent(out AdminToys.SpeakerToy toy))
            throw new MissingComponentException(exceptionMessage);
        return SpeakerToy.Get(toy);
    }

}
