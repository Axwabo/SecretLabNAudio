using SecretLabNAudio.Core.SendEngines;

namespace SecretLabNAudio.Core.Extensions;

public static partial class PersonalizationExtensions
{

    public static SpeakerPersonalization AddPersonalization(this AudioPlayer player)
        => player.gameObject.AddComponent<SpeakerPersonalization>();

    public static SpeakerPersonalization AddPersonalization(this AudioPlayer player, Action<SpeakerPersonalization> configure)
    {
        var personalization = player.AddPersonalization();
        configure(personalization);
        return personalization;
    }

    public static AudioPlayer WithPersonalization(this AudioPlayer player)
    {
        player.AddPersonalization();
        return player;
    }

    public static AudioPlayer WithPersonalization(this AudioPlayer player, Action<SpeakerPersonalization> configure)
    {
        player.AddPersonalization(configure);
        return player;
    }

    public static AudioPlayer WithLivePersonalizedSendEngine(this AudioPlayer player, SpeakerPersonalization personalization, PersonalizedSettingsTransform transform, SendEngine? baseEngine = null)
    {
        player.SendEngine = new LivePersonalizedSendEngine(
            baseEngine ?? player.SendEngine ?? new SendEngine(),
            personalization,
            transform
        );
        return player;
    }

}
