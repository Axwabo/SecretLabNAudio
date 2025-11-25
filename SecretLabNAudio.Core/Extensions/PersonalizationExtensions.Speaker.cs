namespace SecretLabNAudio.Core.Extensions;

public static partial class PersonalizationExtensions
{

    /// <param name="speaker">The speaker to add the personalization to.</param>
    extension(SpeakerToy speaker)
    {

        /// <summary>Gets or adds a <see cref="SpeakerPersonalization"/> component to the <see cref="SpeakerToy"/>.</summary>
        /// <returns>The <see cref="SpeakerPersonalization"/> component attached to the speaker.</returns>
        public SpeakerPersonalization AddPersonalization()
            => speaker.GameObject.TryGetComponent(out SpeakerPersonalization existing)
                ? existing
                : speaker.GameObject.AddComponent<SpeakerPersonalization>();

        /// <summary>
        /// Adds a <see cref="SpeakerPersonalization"/> component to the <see cref="SpeakerToy"/> and configures it using the provided action.
        /// </summary>
        /// <param name="configure">An action to configure the <see cref="SpeakerPersonalization"/>.</param>
        /// <returns>The <see cref="SpeakerPersonalization"/> component attached to the speaker.</returns>
        public SpeakerPersonalization AddPersonalization(Action<SpeakerPersonalization> configure)
        {
            var personalization = speaker.AddPersonalization();
            configure(personalization);
            return personalization;
        }

        /// <summary>
        /// Adds a <see cref="SpeakerPersonalization"/> component to the <see cref="SpeakerToy"/> and returns the speaker itself.
        /// </summary>
        /// <returns>The <see cref="SpeakerToy"/> itself with the personalization added.</returns>
        public SpeakerToy WithPersonalization()
        {
            speaker.AddPersonalization();
            return speaker;
        }

        /// <summary>
        /// Adds a <see cref="SpeakerPersonalization"/> component to the <see cref="SpeakerToy"/> and configures it using the provided action.
        /// </summary>
        /// <param name="configure">An action to configure the <see cref="SpeakerPersonalization"/>.</param>
        /// <returns>The <see cref="SpeakerToy"/> itself with the personalization added and configured.</returns>
        public SpeakerToy WithPersonalization(Action<SpeakerPersonalization> configure)
        {
            speaker.AddPersonalization(configure);
            return speaker;
        }

    }

}
