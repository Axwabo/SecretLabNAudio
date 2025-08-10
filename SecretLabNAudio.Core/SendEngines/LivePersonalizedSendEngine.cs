using VoiceChat.Networking;

namespace SecretLabNAudio.Core.SendEngines;

/// <summary>A delegate that maps a <see cref="SpeakerSettings"/> struct to another per player.</summary>
/// <param name="player">The player map get the settings for.</param>
/// <param name="current">The current settings for the player. <see langword="null"/> if there is no override.</param>
/// <returns>The modified <see cref="SpeakerSettings"/> or <see langword="null"/> if the speaker's original settings should be used.</returns>
/// <seealso cref="SpeakerSettings.From(SpeakerPersonalization)"/>
public delegate SpeakerSettings? PersonalizedSettingsTransform(Player player, SpeakerSettings? current);

/// <summary>Updates the personalized settings on each broadcast per player.</summary>
public class LivePersonalizedSendEngine : SendEngine
{

    private readonly SendEngine _baseEngine;
    private readonly SpeakerPersonalization _personalization;
    private readonly PersonalizedSettingsTransform _settingsTransform;

    /// <summary>Creates a new <see cref="LivePersonalizedSendEngine"/>.</summary>
    /// <param name="baseEngine">The engine used to specify whether the message should be broadcasted.</param>
    /// <param name="personalization">The <see cref="SpeakerPersonalization"/> component to use for the settings.</param>
    /// <param name="settingsTransform">The delegate that maps the current settings to new ones per player.</param>
    public LivePersonalizedSendEngine(SendEngine baseEngine, SpeakerPersonalization personalization, PersonalizedSettingsTransform settingsTransform)
    {
        _baseEngine = baseEngine;
        _personalization = personalization;
        _settingsTransform = settingsTransform;
    }

    /// <inheritdoc />
    protected internal override bool Broadcast(Player player, AudioMessage message)
    {
        if (!_baseEngine.Broadcast(player, message))
            return false;
        _personalization.Modify(player, settings => _settingsTransform(player, settings));
        return true;
    }

}
