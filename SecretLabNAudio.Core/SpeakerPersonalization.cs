using System.Collections.Generic;

namespace SecretLabNAudio.Core;

/// <summary>A delegate that maps a <see cref="SpeakerSettings"/> struct to another.</summary>
/// <param name="current">The current settings for the player. <see langword="null"/> if there is no override.</param>
/// <returns>The modified <see cref="SpeakerSettings"/> or <see langword="null"/> if the default settings should be used.</returns>
/// <seealso cref="SpeakerSettings.From(SpeakerPersonalization)"/>
public delegate SpeakerSettings? SettingsTransform(SpeakerSettings? current);

/// <summary>
/// A <see cref="SpeakerToy"/>-bound component that personalizes audio settings per <see cref="Player"/> wrapper.
/// Persists overrides even when the <see cref="Speaker"/>'s settings are modified.
/// </summary>
/// <see cref="SendEngines.LivePersonalizedSendEngine"/>
/// <remarks>Reconnecting players are not handled.</remarks>
public sealed class SpeakerPersonalization : MonoBehaviour
{

    private readonly Dictionary<Player, SpeakerSettings> _settingsPerUserId = [];

    private SpeakerSettings _previousSettings;

    /// <summary>The <see cref="SpeakerToy"/> this component is attached to.</summary>
    public SpeakerToy Speaker { get; private set; } = null!;

    /// <summary>Gets the personalized settings for the given <see cref="Player"/>.</summary>
    /// <param name="player">The player to get the settings of.</param>
    /// <returns>The settings if any specified; <see langword="null"/> otherwise.</returns>
    public SpeakerSettings? this[Player player]
        => _settingsPerUserId.TryGetValue(player, out var settings) ? settings : null;

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
        var defaultSettings = SpeakerSettings.From(Speaker);
        var settingsToSend = settings ?? defaultSettings;
        if (settings.HasValue)
            _settingsPerUserId[player] = settingsToSend;
        else
            _settingsPerUserId.Remove(player);
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

    private void ResyncAll(SpeakerSettings previousSettings)
    {
        foreach (var kvp in _settingsPerUserId)
            if (kvp.Key.ReferenceHub)
                SendSyncVars(kvp.Key, previousSettings, kvp.Value);
    }

    private void Awake()
    {
        Speaker = this.GetSpeaker("SpeakerPersonalization must be attached to a SpeakerToy.");
        _previousSettings = SpeakerSettings.From(Speaker);
    }

    private void LateUpdate()
    {
        var currentSettings = SpeakerSettings.From(Speaker);
        if (_previousSettings == currentSettings)
            return;
        ResyncAll(_previousSettings);
        _previousSettings = currentSettings;
    }

    private void OnDisable() => _settingsPerUserId.Clear();

    private void OnDestroy() => _settingsPerUserId.Clear();

}
