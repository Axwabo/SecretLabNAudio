using LabApi.Features.Wrappers;

namespace SecretLabNAudio.Core.Extensions;

public static class SpeakerToyExtensions
{

    public static SpeakerToy ApplySettings(this SpeakerToy speaker, SpeakerSettings settings)
    {
        speaker.IsSpatial = settings.IsSpatial;
        speaker.Volume = settings.Volume;
        speaker.MinDistance = settings.MinDistance;
        speaker.MaxDistance = settings.MaxDistance;
        return speaker;
    }

    public static SpeakerToy WithId(this SpeakerToy speaker, byte id)
    {
        speaker.ControllerId = id;
        return speaker;
    }

    public static SpeakerToy WithVolume(this SpeakerToy speaker, float volume)
    {
        speaker.Volume = volume;
        return speaker;
    }

    public static SpeakerToy WithMinDistance(this SpeakerToy speaker, float minDistance)
    {
        speaker.MinDistance = minDistance;
        return speaker;
    }

    public static SpeakerToy WithMaxDistance(this SpeakerToy speaker, float maxDistance)
    {
        speaker.MaxDistance = maxDistance;
        return speaker;
    }

    public static SpeakerToy WithSpatial(this SpeakerToy speaker, bool isSpatial = true)
    {
        speaker.IsSpatial = isSpatial;
        return speaker;
    }

    public static AudioPlayer AddAudioPlayer(this SpeakerToy speaker)
        => speaker.GameObject.TryGetComponent(out AudioPlayer existing)
            ? existing
            : speaker.GameObject.AddComponent<AudioPlayer>();

}
