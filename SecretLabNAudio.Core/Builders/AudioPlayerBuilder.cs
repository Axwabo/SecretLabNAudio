using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Pools;

namespace SecretLabNAudio.Core.Builders;

// TODO: extend ugh
public readonly struct AudioPlayerBuilder : ISpeakerBuilder
{

    public AudioPlayer Player { get; }
    public SpeakerToy Speaker { get; }

    public static AudioPlayerBuilder Create(SpeakerSettings settings, Vector3 position = default, Transform? parent = null)
        => Create(SpeakerToyPool.NextAvailableId, settings, position, parent);

    public static AudioPlayerBuilder Create(byte id, SpeakerSettings settings, Vector3 position = default, Transform? parent = null)
    {
        var speaker = SpeakerToy.Create(position, parent, false)
            .WithId(id)
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
