namespace SecretLabNAudio.Core;

public readonly record struct SpeakerSettings
{

    public static SpeakerSettings Default { get; } = new();

    public static SpeakerSettings From(AudioPlayer player) => From(player.Speaker);

    public static SpeakerSettings From(SpeakerToy speaker) => new()
    {
        IsSpatial = speaker.IsSpatial,
        Volume = speaker.Volume,
        MinDistance = speaker.MinDistance,
        MaxDistance = speaker.MaxDistance
    };

    public SpeakerSettings()
    {
    }

    public bool IsSpatial { get; init; } = true;

    public float Volume { get; init; } = 1;

    public float MinDistance { get; init; } = 1;

    public float MaxDistance { get; init; } = 15;

}
