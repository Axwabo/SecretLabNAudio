using SecretLabNAudio.Core.Outputs;

namespace SecretLabNAudio.Core.Builders;

public readonly struct AudioPlayerBuilder : IAudioPlayerBuilder, ISpeakerBuilder
{

    public AudioPlayer Player { get; }

    public SpeakerToyOutput Output { get; }

    public static AudioPlayerBuilder Create(SpeakerSettings settings, Vector3 position = default, Transform? parent = null, bool spawn = true)
        => new(IAudioPlayerBaseBuilder.Create<AudioPlayer>(settings, position, parent, spawn));

    public static AudioPlayerBuilder Create(byte id, SpeakerSettings settings, Vector3 position = default, Transform? parent = null, bool spawn = true)
        => new(IAudioPlayerBaseBuilder.Create<AudioPlayer>(id, settings, position, parent, spawn));

    public static AudioPlayerBuilder CreatePrivate(Player target, float volume = 1, bool spawn = true)
        => new(IAudioPlayerBaseBuilder.CreatePrivate<AudioPlayer>(target, volume, spawn));

    private AudioPlayerBuilder((AudioPlayer, SpeakerToyOutput) tuple) => (Player, Output) = tuple;

}
