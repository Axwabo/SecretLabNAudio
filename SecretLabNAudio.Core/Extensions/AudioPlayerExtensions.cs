using Axwabo.Helpers;
using NAudio.Wave;

namespace SecretLabNAudio.Core.Extensions;

public static class AudioPlayerExtensions
{

    public static T ApplySettings<T>(this T player, AudioPlayerSettings settings) where T : AudioPlayer
    {
        player.IsSpatial = settings.IsSpatial;
        player.Volume = settings.Volume;
        player.MinDistance = settings.MinDistance;
        player.MaxDistance = settings.MaxDistance;
        return player;
    }

    public static T WithVolume<T>(this T player, float volume) where T : AudioPlayer
    {
        player.Volume = volume;
        return player;
    }

    public static T WithMinDistance<T>(this T player, float minDistance) where T : AudioPlayer
    {
        player.MinDistance = minDistance;
        return player;
    }

    public static T WithMaxDistance<T>(this T player, float maxDistance) where T : AudioPlayer
    {
        player.MaxDistance = maxDistance;
        return player;
    }

    public static T WithSpatial<T>(this T player, bool isSpatial = true) where T : AudioPlayer
    {
        player.IsSpatial = isSpatial;
        return player;
    }

    public static T WithProvider<T>(this T player, ISampleProvider? provider) where T : AudioPlayer
    {
        player.SampleProvider = provider?.ToPlayerCompatible();
        return player;
    }

    public static T Pause<T>(this T player, bool pause = true) where T : AudioPlayer
    {
        player.IsPaused = pause;
        return player;
    }

    public static AudioPlayerPersonalization AddPersonalization(this AudioPlayer player)
        => player.AddPersonalization<AudioPlayerPersonalization>();

    public static TPersonalization AddPersonalization<TPersonalization>(this AudioPlayer player) where TPersonalization : AudioPlayerPersonalization
        => typeof(TPersonalization).IsAbstract
            ? throw new InvalidOperationException("Cannot create an abstract AudioPlayerPersonalization component")
            : player.GetOrAddComponent<TPersonalization>();

    public static TPersonalization AddPersonalization<TPersonalization>(this AudioPlayer player, Action<TPersonalization> configure)
        where TPersonalization : AudioPlayerPersonalization
    {
        var personalization = player.AddPersonalization<TPersonalization>();
        configure.Invoke(personalization);
        return personalization;
    }

    public static TPlayer WithPersonalization<TPlayer>(this TPlayer player) where TPlayer : AudioPlayer
    {
        player.AddPersonalization();
        return player;
    }

    public static TPlayer WithPersonalization<TPlayer, TPersonalization>(this TPlayer player)
        where TPlayer : AudioPlayer
        where TPersonalization : AudioPlayerPersonalization
    {
        player.AddPersonalization<TPersonalization>();
        return player;
    }

    public static TPlayer WithPersonalization<TPlayer, TPersonalization>(this TPlayer player, Action<TPersonalization> configure)
        where TPlayer : AudioPlayer
        where TPersonalization : AudioPlayerPersonalization
    {
        player.AddPersonalization(configure);
        return player;
    }

}
