namespace SecretLabNAudio.Core.Extensions;

internal static class MonoBehaviorExtensions
{

    public static SpeakerToy GetSpeaker(this MonoBehaviour behavior, string exceptionMessage)
    {
        if (!behavior.TryGetComponent(out AdminToys.SpeakerToy toy))
            throw new MissingComponentException(exceptionMessage);
        return SpeakerToy.Get(toy);
    }

}
