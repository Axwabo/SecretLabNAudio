namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for the <see cref="SpeakerToy"/> wrapper.</summary>
public static class SpeakerToyExtensions
{

    /// <param name="speaker">The speaker to apply the settings to.</param>
    extension(SpeakerToy speaker)
    {

        /// <summary>
        /// Applies the given <see cref="SpeakerSettings"/> to the <see cref="SpeakerToy"/>.
        /// </summary>
        /// <param name="settings">The settings to apply.</param>
        /// <returns>The <paramref name="speaker"/> itself.</returns>
        public SpeakerToy ApplySettings(SpeakerSettings settings)
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
        /// <param name="id">The ID to set.</param>
        /// <returns>The <paramref name="speaker"/> itself.</returns>
        public SpeakerToy WithId(byte id)
        {
            speaker.ControllerId = id;
            return speaker;
        }

        /// <summary>
        /// Sets the volume of the <see cref="SpeakerToy"/>.
        /// </summary>
        /// <param name="volume">The volume to set.</param>
        /// <returns>The <paramref name="speaker"/> itself.</returns>
        public SpeakerToy WithVolume(float volume)
        {
            speaker.Volume = volume;
            return speaker;
        }

        /// <summary>
        /// Sets the minimum full volume distance of the <see cref="SpeakerToy"/>.
        /// </summary>
        /// <param name="minDistance">The minimum distance to set.</param>
        /// <returns>The <paramref name="speaker"/> itself.</returns>
        public SpeakerToy WithMinDistance(float minDistance)
        {
            speaker.MinDistance = minDistance;
            return speaker;
        }

        /// <summary>
        /// Sets the maximum audible distance of the <see cref="SpeakerToy"/>.
        /// </summary>
        /// <param name="maxDistance">The maximum distance to set.</param>
        /// <returns>The <paramref name="speaker"/> itself.</returns>
        public SpeakerToy WithMaxDistance(float maxDistance)
        {
            speaker.MaxDistance = maxDistance;
            return speaker;
        }

        /// <summary>
        /// Sets whether the <see cref="SpeakerToy"/> should use spatial audio.
        /// </summary>
        /// <param name="isSpatial">Whether the speaker should use spatial audio.</param>
        /// <returns>The <paramref name="speaker"/> itself.</returns>
        public SpeakerToy WithSpatial(bool isSpatial = true)
        {
            speaker.IsSpatial = isSpatial;
            return speaker;
        }

        /// <summary>
        /// Gets or adds an <see cref="AudioPlayer"/> component to the <see cref="SpeakerToy"/>.
        /// </summary>
        /// <returns>The <see cref="AudioPlayer"/> component attached to the speaker.</returns>
        public AudioPlayer AddAudioPlayer()
            => speaker.GameObject.TryGetComponent(out AudioPlayer existing)
                ? existing
                : speaker.GameObject.AddComponent<AudioPlayer>();

    }

}
