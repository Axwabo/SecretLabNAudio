namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="SpeakerToy"/> wrapper.</summary>
public static class SpeakerToyExtensions
{

    /// <summary>
    /// Applies the given <see cref="SpeakerSettings"/> to the <see cref="SpeakerToy"/>.
    /// </summary>
    /// <param name="speaker">The speaker to apply the settings to.</param>
    /// <param name="settings">The settings to apply.</param>
    /// <returns>The <paramref name="speaker"/> itself.</returns>
    public static SpeakerToy ApplySettings(this SpeakerToy speaker, SpeakerSettings settings)
    {
        speaker.IsSpatial = settings.IsSpatial;
        speaker.Volume = settings.Volume;
        speaker.MinDistance = settings.MinDistance;
        speaker.MaxDistance = settings.MaxDistance;
        return speaker;
    }

    /// <summary>
    /// Sets the controller ID of the <see cref="SpeakerToy"/>.
    /// </summary>
    /// <param name="speaker">The speaker to set the ID of.</param>
    /// <param name="id">The ID to set.</param>
    /// <returns>The <paramref name="speaker"/> itself.</returns>
    public static SpeakerToy WithId(this SpeakerToy speaker, byte id)
    {
        speaker.ControllerId = id;
        return speaker;
    }

    /// <summary>
    /// Sets the volume of the <see cref="SpeakerToy"/>.
    /// </summary>
    /// <param name="speaker">The speaker to set the volume of.</param>
    /// <param name="volume">The volume to set.</param>
    /// <returns>The <paramref name="speaker"/> itself.</returns>
    public static SpeakerToy WithVolume(this SpeakerToy speaker, float volume)
    {
        speaker.Volume = volume;
        return speaker;
    }

    /// <summary>
    /// Sets the minimum full volume distance of the <see cref="SpeakerToy"/>.
    /// </summary>
    /// <param name="speaker">The speaker to set the minimum distance of.</param>
    /// <param name="minDistance">The minimum distance to set.</param>
    /// <returns>The <paramref name="speaker"/> itself.</returns>
    public static SpeakerToy WithMinDistance(this SpeakerToy speaker, float minDistance)
    {
        speaker.MinDistance = minDistance;
        return speaker;
    }

    /// <summary>
    /// Sets the maximum audible distance of the <see cref="SpeakerToy"/>.
    /// </summary>
    /// <param name="speaker">The speaker to set the maximum distance of.</param>
    /// <param name="maxDistance">The maximum distance to set.</param>
    /// <returns>The <paramref name="speaker"/> itself.</returns>
    public static SpeakerToy WithMaxDistance(this SpeakerToy speaker, float maxDistance)
    {
        speaker.MaxDistance = maxDistance;
        return speaker;
    }

    /// <summary>
    /// Sets whether the <see cref="SpeakerToy"/> should use spatial audio.
    /// </summary>
    /// <param name="speaker">The speaker to set the spatial audio setting of.</param>
    /// <param name="isSpatial">Whether the speaker should use spatial audio.</param>
    /// <returns>The <paramref name="speaker"/> itself.</returns>
    public static SpeakerToy WithSpatial(this SpeakerToy speaker, bool isSpatial = true)
    {
        speaker.IsSpatial = isSpatial;
        return speaker;
    }

    /// <summary>
    /// Gets or adds an <see cref="AudioPlayer"/> component to the <see cref="SpeakerToy"/>.
    /// </summary>
    /// <param name="speaker">The speaker to add the audio player to.</param>
    /// <returns>The <see cref="AudioPlayer"/> component attached to the speaker.</returns>
    public static AudioPlayer AddAudioPlayer(this SpeakerToy speaker)
        => speaker.GameObject.TryGetComponent(out AudioPlayer existing)
            ? existing
            : speaker.GameObject.AddComponent<AudioPlayer>();

}
