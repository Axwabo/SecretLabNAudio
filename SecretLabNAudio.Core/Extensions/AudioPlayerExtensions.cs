using Axwabo.Helpers;
using NAudio.Wave;

namespace SecretLabNAudio.Core.Extensions;

public static class AudioPlayerExtensions
{

    public static AudioPlayer ApplySettings(this AudioPlayer player, AudioPlayerSettings settings)
    {
        player.IsSpatial = settings.IsSpatial;
        player.Volume = settings.Volume;
        player.MinDistance = settings.MinDistance;
        player.MaxDistance = settings.MaxDistance;
        return player;
    }

    public static AudioPlayer WithVolume(this AudioPlayer player, float volume)
    {
        player.Volume = volume;
        return player;
    }

    public static AudioPlayer WithMinDistance(this AudioPlayer player, float minDistance)
    {
        player.MinDistance = minDistance;
        return player;
    }

    public static AudioPlayer WithMaxDistance(this AudioPlayer player, float maxDistance)
    {
        player.MaxDistance = maxDistance;
        return player;
    }

    public static AudioPlayer WithSpatial(this AudioPlayer player, bool isSpatial = true)
    {
        player.IsSpatial = isSpatial;
        return player;
    }

    public static AudioPlayer WithProvider(this AudioPlayer player, ISampleProvider? provider)
    {
        player.SampleProvider = provider?.ToPlayerCompatible();
        return player;
    }

    public static AudioPlayer Pause(this AudioPlayer player, bool pause = true)
    {
        player.IsPaused = pause;
        return player;
    }

    public static AudioPlayerPersonalization AddPersonalization(this AudioPlayer player)
        => player.AddPersonalization<AudioPlayerPersonalization>();

    public static T AddPersonalization<T>(this AudioPlayer player) where T : AudioPlayerPersonalization
        => typeof(T).IsAbstract
            ? throw new InvalidOperationException("Cannot create an abstract AudioPlayerPersonalization component")
            : player.GetOrAddComponent<T>();

    public static T AddPersonalization<T>(this AudioPlayer player, Action<T> configure) where T : AudioPlayerPersonalization
    {
        var personalization = player.AddPersonalization<T>();
        configure.Invoke(personalization);
        return personalization;
    }

    public static AudioPlayer WithPersonalization(this AudioPlayer player)
    {
        player.AddPersonalization();
        return player;
    }

    public static AudioPlayer WithPersonalization<T>(this AudioPlayer player) where T : AudioPlayerPersonalization
    {
        player.AddPersonalization<T>();
        return player;
    }

    public static AudioPlayer WithPersonalization<T>(this AudioPlayer player, Action<T> configure) where T : AudioPlayerPersonalization
    {
        player.AddPersonalization(configure);
        return player;
    }

}
