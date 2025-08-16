using SecretLabNAudio.Core.SendEngines;

namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods to manage <see cref="SpeakerPersonalization"/> components.</summary>
public static partial class PersonalizationExtensions
{

    /// <summary>Gets or adds a <see cref="SpeakerPersonalization"/> component to the <see cref="AudioPlayer"/>.</summary>
    /// <param name="player">The player to add the personalization to.</param>
    /// <returns>The <see cref="SpeakerPersonalization"/> component attached to the player.</returns>
    public static SpeakerPersonalization AddPersonalization(this AudioPlayer player)
        => player.TryGetComponent(out SpeakerPersonalization existing)
            ? existing
            : player.gameObject.AddComponent<SpeakerPersonalization>();

    /// <summary>
    /// Adds a <see cref="SpeakerPersonalization"/> component to the <see cref="AudioPlayer"/> and configures it using the provided action.
    /// </summary>
    /// <param name="player">The player to add the personalization to.</param>
    /// <param name="configure">An action to configure the <see cref="SpeakerPersonalization"/>.</param>
    /// <returns>The <see cref="SpeakerPersonalization"/> component attached to the player.</returns>
    public static SpeakerPersonalization AddPersonalization(this AudioPlayer player, Action<SpeakerPersonalization> configure)
    {
        var personalization = player.AddPersonalization();
        configure(personalization);
        return personalization;
    }

    /// <summary>
    /// Adds a <see cref="SpeakerPersonalization"/> component to the <see cref="AudioPlayer"/> and returns the player itself.
    /// </summary>
    /// <param name="player">The player to add the personalization to.</param>
    /// <returns>The <see cref="AudioPlayer"/> itself with the personalization added.</returns>
    public static AudioPlayer WithPersonalization(this AudioPlayer player)
    {
        player.AddPersonalization();
        return player;
    }

    /// <summary>
    /// Adds a <see cref="SpeakerPersonalization"/> component to the <see cref="AudioPlayer"/> and configures it using the provided action.
    /// </summary>
    /// <param name="player">The player to add the personalization to.</param>
    /// <param name="configure">An action to configure the <see cref="SpeakerPersonalization"/>.</param>
    /// <returns>The <see cref="AudioPlayer"/> itself with the personalization added and configured.</returns>
    public static AudioPlayer WithPersonalization(this AudioPlayer player, Action<SpeakerPersonalization> configure)
    {
        player.AddPersonalization(configure);
        return player;
    }

    /// <summary>
    /// Sets the <see cref="AudioPlayer.SendEngine"/> of the <see cref="AudioPlayer"/> to a <see cref="LivePersonalizedSendEngine"/> based on the given <see cref="SpeakerPersonalization"/> component.
    /// </summary>
    /// <param name="player">The player to set the send engine for.</param>
    /// <param name="personalization">The <see cref="SpeakerPersonalization"/> component to use for personalization.</param>
    /// <param name="transform">A delegate that transforms the personalized settings.</param>
    /// <param name="baseEngine">
    /// The base engine used to construct the <see cref="LivePersonalizedSendEngine"/>.
    /// If <see langword="null"/>, <see cref="SendEngine.DefaultEngine"/> will be used.
    /// </param>
    /// <returns>The <see cref="AudioPlayer"/> itself with the personalized send engine set.</returns>
    /// <seealso cref="PersonalizedSettingsTransform"/>
    public static AudioPlayer WithLivePersonalizedSendEngine(this AudioPlayer player, SpeakerPersonalization personalization, PersonalizedSettingsTransform transform, SendEngine? baseEngine = null)
    {
        player.SendEngine = new LivePersonalizedSendEngine(
            baseEngine ?? SendEngine.DefaultEngine,
            personalization,
            transform
        );
        return player;
    }

}
