using SecretLabNAudio.Core.Pools;
using SecretLabNAudio.Core.SendEngines;

namespace SecretLabNAudio.Core.Extensions;

public static class AudioPlayerExtensions
{

    private static AudioPlayer PatchSpeaker<T>(this AudioPlayer player, Func<SpeakerToy, T, SpeakerToy> method, T parameter)
    {
        method(player.Speaker, parameter);
        return player;
    }

    public static AudioPlayer ApplySettings(this AudioPlayer player, SpeakerSettings settings)
        => player.PatchSpeaker(SpeakerToyExtensions.ApplySettings, settings);

    public static AudioPlayer WithId(this AudioPlayer player, byte id)
        => player.PatchSpeaker(SpeakerToyExtensions.WithId, id);

    public static AudioPlayer WithVolume(this AudioPlayer player, float volume)
        => player.PatchSpeaker(SpeakerToyExtensions.WithVolume, volume);

    public static AudioPlayer WithMinDistance(this AudioPlayer player, float minDistance)
        => player.PatchSpeaker(SpeakerToyExtensions.WithMinDistance, minDistance);

    public static AudioPlayer WithMaxDistance(this AudioPlayer player, float maxDistance)
        => player.PatchSpeaker(SpeakerToyExtensions.WithMaxDistance, maxDistance);

    public static AudioPlayer WithSpatial(this AudioPlayer player, bool isSpatial = true)
        => player.PatchSpeaker(SpeakerToyExtensions.WithSpatial, isSpatial);

    public static AudioPlayer WithProvider(this AudioPlayer player, ISampleProvider? provider)
    {
        player.SampleProvider = provider?.ToPlayerCompatible();
        return player;
    }

    public static AudioPlayer Buffer(this AudioPlayer player, double seconds)
    {
        if (player.SampleProvider == null)
            return player;
        player.SampleProvider = player.SampleProvider.Buffer(seconds);
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

    public static AudioPlayer UnsetProviderOnEnd(this AudioPlayer player)
    {
        player.NoSamplesRead += () => player.SampleProvider = null;
        return player;
    }

    public static AudioPlayer DestroyOnEnd(this AudioPlayer player)
    {
        player.UnsetProviderOnEnd();
        player.NoSamplesRead += () => NetworkServer.Destroy(player.gameObject);
        return player;
    }

    public static AudioPlayer PoolOnEnd(this AudioPlayer player)
    {
        player.NoSamplesRead += () => AudioPlayerPool.Return(player);
        return player;
    }

}
