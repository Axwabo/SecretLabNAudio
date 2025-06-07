using System.Collections.Generic;
using LabApi.Features.Wrappers;
using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.Core;

public sealed class SpeakerPersonalization : MonoBehaviour
{

    public SpeakerToy Speaker { get; private set; } = null!;

    private void Awake() => Speaker = this.GetSpeaker("SpeakerPersonalization must be attached to a SpeakerToy.");

    private readonly Dictionary<string, SpeakerSettings> _settingsPerUserId = [];

    public SpeakerSettings? this[ReferenceHub hub] => this[hub.NotNullUserId()];

    public SpeakerSettings? this[string userId]
        => _settingsPerUserId.TryGetValue(userId, out var settings) ? settings : null;

    public void Override(ReferenceHub hub, SpeakerSettings settings) => Sync(hub, this[hub], settings);

    public void Modify(ReferenceHub hub, Func<SpeakerSettings?, SpeakerSettings?> settingsTransform)
    {
        var current = this[hub];
        Sync(hub, current, settingsTransform(current));
    }

    public void ClearOverride(ReferenceHub hub) => Sync(hub, this[hub], null);

    private void Sync(ReferenceHub hub, SpeakerSettings? previous, SpeakerSettings? settings)
    {
        if (previous == settings)
            return;
        var id = hub.NotNullUserId();
        var defaultSettings = SpeakerSettings.From(Speaker);
        var settingsToSend = settings ?? defaultSettings;
        if (settings.HasValue)
            _settingsPerUserId[id] = settingsToSend;
        else
            _settingsPerUserId.Remove(id);
        var actualPrevious = previous ?? defaultSettings;
        SendSyncVars(hub, actualPrevious, settingsToSend);
    }

    private void SendSyncVars(ReferenceHub hub, SpeakerSettings previous, SpeakerSettings current)
        => SpeakerSyncVars.SendFakeSyncVars(hub.connectionToClient, Speaker.Base, (
            Mathf.Approximately(previous.Volume, current.Volume) ? null : current.Volume,
            previous.IsSpatial == current.IsSpatial ? null : current.IsSpatial,
            Mathf.Approximately(previous.MinDistance, current.MinDistance) ? null : current.MinDistance,
            Mathf.Approximately(previous.MaxDistance, current.MaxDistance) ? null : current.MaxDistance
        ));

}
