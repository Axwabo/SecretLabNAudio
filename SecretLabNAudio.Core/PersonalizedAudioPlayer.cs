using System.Collections.Generic;
using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.Core;

public class PersonalizedAudioPlayer : TargetedAudioPlayer
{

    private readonly Dictionary<string, AudioPlayerSettings> _settingsPerUserId = [];

    public AudioPlayerSettings? this[ReferenceHub hub] => this[hub.NotNullUserId()];

    public AudioPlayerSettings? this[string userId]
        => _settingsPerUserId.TryGetValue(userId, out var settings) ? settings : null;

    public void Override(ReferenceHub hub, AudioPlayerSettings settings) => Sync(hub, this[hub], settings);

    public void Modify(ReferenceHub hub, Func<AudioPlayerSettings?, AudioPlayerSettings?> settingsTransform)
    {
        var current = this[hub];
        Sync(hub, current, settingsTransform(current));
    }

    public void ClearOverride(ReferenceHub hub) => Sync(hub, this[hub], null);

    private void Sync(ReferenceHub hub, AudioPlayerSettings? previous, AudioPlayerSettings? settings)
    {
        if (previous == settings)
            return;
        var id = hub.NotNullUserId();
        var defaultSettings = AudioPlayerSettings.From(this);
        var settingsToSend = settings ?? defaultSettings;
        if (settings.HasValue)
            _settingsPerUserId[id] = settingsToSend;
        else
            _settingsPerUserId.Remove(id);
        var actualPrevious = previous ?? defaultSettings;
        SendSyncVars(hub, actualPrevious, settingsToSend);
    }

    private void SendSyncVars(ReferenceHub hub, in AudioPlayerSettings previous, in AudioPlayerSettings current)
        => SpeakerToyExtensions.SendFakeSyncVars(hub.connectionToClient, Speaker, (
            Mathf.Approximately(previous.Volume, current.Volume) ? null : current.Volume,
            previous.IsSpatial == current.IsSpatial ? null : current.IsSpatial,
            Mathf.Approximately(previous.MinDistance, current.MinDistance) ? null : current.MinDistance,
            Mathf.Approximately(previous.MaxDistance, current.MaxDistance) ? null : current.MaxDistance
        ));

}
