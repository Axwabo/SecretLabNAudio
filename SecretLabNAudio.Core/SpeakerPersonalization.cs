using System.Collections.Generic;

namespace SecretLabNAudio.Core;

/// <summary>A delegate that maps a <see cref="SpeakerSettings"/> struct to another.</summary>
/// <param name="current">The current settings for the player. <see langword="null"/> if there is no override.</param>
/// <returns>The modified <see cref="SpeakerSettings"/> or <see langword="null"/> if the default settings should be used.</returns>
/// <seealso cref="SpeakerSettings.From(SpeakerPersonalization)"/>
public delegate SpeakerSettings? SettingsTransform(SpeakerSettings? current);

/// <summary>A <see cref="SpeakerToy"/>-bound component </summary>
/// <see cref="SendEngines.LivePersonalizedSendEngine"/>
public sealed class SpeakerPersonalization : MonoBehaviour
{

    /// <summary>The <see cref="SpeakerToy"/> this component is attached to.</summary>
    public SpeakerToy Speaker { get; private set; } = null!;

    private void Awake() => Speaker = this.GetSpeaker("SpeakerPersonalization must be attached to a SpeakerToy.");

    private readonly Dictionary<string, SpeakerSettings> _settingsPerUserId = [];

    /// <summary>Gets the personalized settings for the given <see cref="Player"/>.</summary>
    /// <param name="player">The player to get the settings of.</param>
    /// <returns>The settings if any specified; <see langword="null"/> otherwise.</returns>
    public SpeakerSettings? this[Player player] => this[player.NotNullUserId()];

    /// <summary>Gets the personalized settings for the given <paramref name="userId"/>.</summary>
    /// <param name="userId">The user ID to get the settings of.</param>
    /// <returns>The settings if any specified; <see langword="null"/> otherwise.</returns>
    public SpeakerSettings? this[string userId]
        => _settingsPerUserId.TryGetValue(userId, out var settings) ? settings : null;

    /// <summary>
    /// Overrides the settings for the given <see cref="Player"/>.
    /// </summary>
    /// <param name="player">The player to override the settings for.</param>
    /// <param name="settings">The settings to set.</param>
    public void Override(Player player, SpeakerSettings settings) => Sync(player, this[player], settings);

    /// <summary>
    /// Transforms the settings for the given <see cref="Player"/>.
    /// </summary>
    /// <param name="player">The player to change the settings for.</param>
    /// <param name="settingsTransform">A delegate modifying the settings.</param>
    /// <seealso cref="SettingsTransform"/>
    public void Modify(Player player, SettingsTransform settingsTransform)
    {
        var current = this[player];
        Sync(player, current, settingsTransform(current));
    }

    /// <summary>Clears the settings override for the specified player and sends the default settings.</summary>
    /// <param name="player">The player to clear the settings for.</param>
    public void ClearOverride(Player player) => Sync(player, this[player], null);

    private void Sync(Player player, SpeakerSettings? previous, SpeakerSettings? settings)
    {
        if (previous == settings)
            return;
        var id = player.NotNullUserId();
        var defaultSettings = SpeakerSettings.From(Speaker);
        var settingsToSend = settings ?? defaultSettings;
        if (settings.HasValue)
            _settingsPerUserId[id] = settingsToSend;
        else
            _settingsPerUserId.Remove(id);
        var actualPrevious = previous ?? defaultSettings;
        SendSyncVars(player, actualPrevious, settingsToSend);
    }

    private void SendSyncVars(Player player, SpeakerSettings previous, SpeakerSettings current)
        => SpeakerSyncVars.SendFakeSyncVars(player.Connection, Speaker.Base, (
            Mathf.Approximately(previous.Volume, current.Volume) ? null : current.Volume,
            previous.IsSpatial == current.IsSpatial ? null : current.IsSpatial,
            Mathf.Approximately(previous.MinDistance, current.MinDistance) ? null : current.MinDistance,
            Mathf.Approximately(previous.MaxDistance, current.MaxDistance) ? null : current.MaxDistance
        ));

    private void OnDisable() => _settingsPerUserId.Clear();

}
