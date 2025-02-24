using Axwabo.Helpers;

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

}