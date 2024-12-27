namespace SecretLabNAudio.Core;

public readonly struct AudioPlayerSettings
{

    public static AudioPlayerSettings Default { get; } = new();

    public AudioPlayerSettings()
    {
    }

    public bool IsSpatial { get; init; } = true;

    public float Volume { get; init; } = 1;

    public float MinDistance { get; init; } = 1;

    public float MaxDistance { get; init; } = 15;

}
