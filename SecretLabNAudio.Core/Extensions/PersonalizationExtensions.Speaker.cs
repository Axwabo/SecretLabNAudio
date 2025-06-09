using LabApi.Features.Wrappers;

namespace SecretLabNAudio.Core.Extensions;

public static partial class PersonalizationExtensions
{

    public static SpeakerPersonalization AddPersonalization(this SpeakerToy speaker)
        => speaker.GameObject.TryGetComponent(out SpeakerPersonalization existing)
            ? existing
            : speaker.GameObject.AddComponent<SpeakerPersonalization>();

    public static SpeakerPersonalization AddPersonalization(this SpeakerToy speaker, Action<SpeakerPersonalization> configure)
    {
        var personalization = speaker.AddPersonalization();
        configure(personalization);
        return personalization;
    }

    public static SpeakerToy WithPersonalization(this SpeakerToy speaker)
    {
        speaker.AddPersonalization();
        return speaker;
    }

    public static SpeakerToy WithPersonalization(this SpeakerToy speaker, Action<SpeakerPersonalization> configure)
    {
        speaker.AddPersonalization(configure);
        return speaker;
    }

}
