namespace SecretLabNAudio.Core.Extensions;

public static partial class PersonalizationExtensions
{

    extension(SpeakerPersonalization personalization)
    {

        /// <summary>
        /// Removes overrides for connected players.
        /// </summary>
        /// <seealso cref="SpeakerPersonalization.ClearOverride"/>
        /// <seealso cref="Player.ReadyList"/>
        public void ClearAllOverrides()
        {
            foreach (var player in Player.ReadyList)
                personalization.ClearOverride(player);
        }

    }

    /// <param name="enumerable">The instances to modify.</param>
    extension(IEnumerable<SpeakerPersonalization> enumerable)
    {

        /// <inheritdoc cref="SpeakerPersonalization.Override"/>
        public void Override(Player player, SpeakerSettings settings)
        {
            foreach (var personalization in enumerable)
                personalization.Override(player, settings);
        }

        /// <inheritdoc cref="SpeakerPersonalization.Modify"/>
        public void Modify(Player player, SettingsTransform settingsTransform)
        {
            foreach (var personalization in enumerable)
                personalization.Modify(player, settingsTransform);
        }

        /// <inheritdoc cref="SpeakerPersonalization.ClearOverride"/>
        public void ClearOverride(Player player)
        {
            foreach (var personalization in enumerable)
                personalization.ClearOverride(player);
        }

        /// <summary>
        /// Removes overrides for connected players on each personalization instance.
        /// </summary>
        public void ClearAllOverrides()
        {
            foreach (var personalization in enumerable)
                personalization.ClearAllOverrides();
        }

    }

}
