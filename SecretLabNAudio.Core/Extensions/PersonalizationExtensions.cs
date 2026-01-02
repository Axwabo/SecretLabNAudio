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

}
