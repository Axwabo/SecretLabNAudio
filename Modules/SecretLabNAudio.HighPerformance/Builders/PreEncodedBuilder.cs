using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Builders;
using SecretLabNAudio.Core.Outputs;

namespace SecretLabNAudio.HighPerformance.Builders;

public readonly struct PreEncodedBuilder : IPreEncodedBuilder, ISpeakerBuilder
{

    public PreEncodedAudioPlayer Player { get; }

    public SpeakerToyOutput Output { get; }

    public static PreEncodedBuilder Create(SpeakerSettings settings, Vector3 position = default, Transform? parent = null, bool spawn = true)
        => new(IAudioPlayerBaseBuilder.Create<PreEncodedAudioPlayer>(settings, position, parent, spawn));

    public static PreEncodedBuilder Create(byte id, SpeakerSettings settings, Vector3 position = default, Transform? parent = null, bool spawn = true)
        => new(IAudioPlayerBaseBuilder.Create<PreEncodedAudioPlayer>(id, settings, position, parent, spawn));

    public static PreEncodedBuilder CreatePrivate(Player target, float volume = 1, bool spawn = true)
        => new(IAudioPlayerBaseBuilder.CreatePrivate<PreEncodedAudioPlayer>(target, volume, spawn));

    private PreEncodedBuilder((PreEncodedAudioPlayer, SpeakerToyOutput) tuple) => (Player, Output) = tuple;

}
