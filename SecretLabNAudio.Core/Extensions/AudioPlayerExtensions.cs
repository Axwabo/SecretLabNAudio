using NAudio.Wave;
using SecretLabNAudio.Core.SendEngines;

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

    public static AudioPlayer WithSendEngine(this AudioPlayer player, SendEngine engine)
    {
        player.SendEngine = engine;
        return player;
    }

    public static AudioPlayer WithFilteredSendEngine(this AudioPlayer player, Predicate<ReferenceHub> filter) 
        => player.WithSendEngine(new FilteredSendEngine(filter));

}
