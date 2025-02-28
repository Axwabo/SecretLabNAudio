using Axwabo.Helpers;
using SecretLabNAudio.Core.SendEngines;

namespace SecretLabNAudio.Core.Extensions;

public static class PersonalizationExtensions
{

    public static AudioPlayerPersonalization AddPersonalization(this AudioPlayer player)
        => player.GetOrAddComponent<AudioPlayerPersonalization>();

    public static AudioPlayerPersonalization AddPersonalization(this AudioPlayer player, Action<AudioPlayerPersonalization> configure)
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

    public static AudioPlayer WithPersonalization(this AudioPlayer player, Action<AudioPlayerPersonalization> configure)
    {
        player.AddPersonalization(configure);
        return player;
    }

    public static AudioPlayer WithLivePersonalizedSendEngine(this AudioPlayer player, AudioPlayerPersonalization personalization, PersonalizedSettingsTransform transform, SendEngine? baseEngine = null)
    {
        player.SendEngine = new LivePersonalizedSendEngine(
            baseEngine ?? player.SendEngine ?? new SendEngine(),
            personalization,
            transform
        );
        return player;
    }

    public static AudioPlayerPersonalization WithLiveSendEngine(this AudioPlayerPersonalization personalization, PersonalizedSettingsTransform transform, SendEngine? baseEngine = null)
    {
        personalization.Player.WithLivePersonalizedSendEngine(personalization, transform, baseEngine);
        return personalization;
    }

}
