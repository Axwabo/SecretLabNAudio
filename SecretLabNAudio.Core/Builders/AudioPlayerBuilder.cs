using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;

namespace SecretLabNAudio.Core.Builders;

// TODO: extend ugh
public readonly struct AudioPlayerBuilder
{

    public readonly AudioPlayer Player;
    public readonly SpeakerToy Speaker;

    public static AudioPlayerBuilder Create(SpeakerSettings settings, Vector3 position = default, Transform? parent = null)
    {
        var speaker = SpeakerToy.Create(position, parent, false)
            .WithId(SpeakerToyPool.NextAvailableId)
            .ApplySettings(settings);
        speaker.Spawn();
        return new AudioPlayerBuilder(speaker.AddAudioPlayer(), speaker);
    }

    private AudioPlayerBuilder(AudioPlayer player, SpeakerToy speaker)
    {
        Player = player;
        Speaker = speaker;
    }

}
