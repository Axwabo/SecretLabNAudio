namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods to manage <see cref="SpeakerPersonalization"/> components.</summary>
public static partial class PersonalizationExtensions
{

    /// <param name="player">The player to add the personalization to.</param>
    extension(AudioPlayer player)
    {

        /// <summary>Gets or adds a <see cref="SpeakerPersonalization"/> component to the <see cref="AudioPlayer"/>.</summary>
        /// <returns>The <see cref="SpeakerPersonalization"/> component attached to the player.</returns>
        public SpeakerPersonalization AddPersonalization()
            => player.TryGetComponent(out SpeakerPersonalization existing)
                ? existing
                : player.gameObject.AddComponent<SpeakerPersonalization>();

        /// <summary>
        /// Adds a <see cref="SpeakerPersonalization"/> component to the <see cref="AudioPlayer"/> and configures it using the provided action.
        /// </summary>
        /// <param name="configure">An action to configure the <see cref="SpeakerPersonalization"/>.</param>
        /// <returns>The <see cref="SpeakerPersonalization"/> component attached to the player.</returns>
        public SpeakerPersonalization AddPersonalization(Action<SpeakerPersonalization> configure)
        {
            var personalization = player.AddPersonalization();
            configure(personalization);
            return personalization;
        }

        /// <summary>
        /// Adds a <see cref="SpeakerPersonalization"/> component to the <see cref="AudioPlayer"/> and returns the player itself.
        /// </summary>
        /// <returns>The <see cref="AudioPlayer"/> itself with the personalization added.</returns>
        public AudioPlayer WithPersonalization()
        {
            player.AddPersonalization();
            return player;
        }

        /// <summary>
        /// Adds a <see cref="SpeakerPersonalization"/> component to the <see cref="AudioPlayer"/> and configures it using the provided action.
        /// </summary>
        /// <param name="configure">An action to configure the <see cref="SpeakerPersonalization"/>.</param>
        /// <returns>The <see cref="AudioPlayer"/> itself with the personalization added and configured.</returns>
        public AudioPlayer WithPersonalization(Action<SpeakerPersonalization> configure)
        {
            player.AddPersonalization(configure);
            return player;
        }

        // TODO: per-frame personalization

    }

}
