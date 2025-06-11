namespace SecretLabNAudio.Core;

/// <summary>Configuration for a SpeakerToy.</summary>
public readonly record struct SpeakerSettings
{

    /// <summary>
    /// Default SpeakerToy settings.
    /// <para>
    /// <see cref="IsSpatial"/> = true<br/>
    /// <see cref="Volume"/> = 1<br/>
    /// <see cref="MinDistance"/> = 1<br/>
    /// <see cref="MaxDistance"/> = 15
    /// </para>
    /// </summary>
    public static SpeakerSettings Default { get; } = new();

    /// <summary>
    /// Gets the settings of the given <see cref="AudioPlayer"/>.
    /// </summary>
    /// <param name="player">The player to get the settings from.</param>
    /// <returns>The settings of the player.</returns>
    public static SpeakerSettings From(AudioPlayer player) => From(player.Speaker);

    /// <summary>
    /// Gets the settings of the given <see cref="SpeakerPersonalization"/>.
    /// </summary>
    /// <param name="personalization">The personalization component to get the settings from.</param>
    /// <returns>The settings of the personalization component.</returns>
    public static SpeakerSettings From(SpeakerPersonalization personalization) => From(personalization.Speaker);

    /// <summary>
    /// Gets the settings of the given <see cref="SpeakerToy"/>.
    /// </summary>
    /// <param name="speaker">The speaker to get the settings from.</param>
    /// <returns>The settings of the speaker.</returns>
    public static SpeakerSettings From(SpeakerToy speaker) => new()
    {
        IsSpatial = speaker.IsSpatial,
        Volume = speaker.Volume,
        MinDistance = speaker.MinDistance,
        MaxDistance = speaker.MaxDistance
    };

    /// <summary>Creates a new <see cref="SpeakerSettings"/> struct with default settings.</summary>
    /// <seealso cref="Default"/>
    public SpeakerSettings()
    {
    }

    /// <summary>Whether the speaker is spatial (has 3D sound).</summary>
    public bool IsSpatial { get; init; } = true;

    /// <summary>The volume of the speaker (range 0-1).</summary>
    public float Volume { get; init; } = 1;

    /// <summary>Minimum distance where attentuation begins. Up until this distance the audio is heard at full volume.</summary>
    public float MinDistance { get; init; } = 1;

    /// <summary>Maximum distance of the speaker. From this distance beyond, the audio is completely inaudible.</summary>
    public float MaxDistance { get; init; } = 15;

}
