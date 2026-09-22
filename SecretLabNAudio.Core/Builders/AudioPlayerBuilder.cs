using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Outputs;
using SecretLabNAudio.Core.Pools;

namespace SecretLabNAudio.Core.Builders;

public readonly struct AudioPlayerBuilder : IAudioPlayerBuilder, ISpeakerBuilder
{

    public AudioPlayer Player { get; }

    public SpeakerToyOutput Output { get; }

    public static AudioPlayerBuilder Create(SpeakerSettings settings, Vector3 position = default, Transform? parent = null, bool spawn = true)
        => Create(SpeakerToyPool.NextAvailableId, settings, position, parent, spawn);

    public static AudioPlayerBuilder Create(byte id, SpeakerSettings settings, Vector3 position = default, Transform? parent = null, bool spawn = true)
    {
        var speaker = SpeakerToy.Create(position, parent, false)
            .WithId(id)
            .ApplySettings(settings);
        var output = new SpeakerToyOutput(speaker);
        var o = speaker.GameObject;
        if (spawn)
            NetworkServer.Spawn(o);
        var player = o.AddComponent<AudioPlayer>().WithPacketOutput(output);
        return new AudioPlayerBuilder(player, output);
    }

    public static AudioPlayerBuilder CreatePrivate(Player target, float volume = 1, bool spawn = true)
        => Create(SpeakerSettings.GloballyAudible, Vector3.zero, target.GameObject!.transform, spawn)
            .WithVolume(volume)
            .WithSendFilter(new SinglePlayerFilter(target));

    private AudioPlayerBuilder(AudioPlayer player, SpeakerToyOutput output)
    {
        Player = player;
        Output = output;
    }

}
