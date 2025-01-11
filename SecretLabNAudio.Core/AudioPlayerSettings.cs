namespace SecretLabNAudio.Core;

public readonly record struct AudioPlayerSettings
{

    public static AudioPlayerSettings Default { get; } = new();

    public static AudioPlayerSettings From(AudioPlayer player) => new()
    {
        IsSpatial = player.IsSpatial,
        Volume = player.Volume,
        MinDistance = player.MinDistance,
        MaxDistance = player.MaxDistance
    };

    public AudioPlayerSettings()
    {
    }

    public bool IsSpatial { get; init; } = true;

    public float Volume { get; init; } = 1;

    public float MinDistance { get; init; } = 1;

    public float MaxDistance { get; init; } = 15;

}
