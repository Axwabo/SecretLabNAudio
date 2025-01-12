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
        var id = hub.NotNullUserId();
        if (settings.HasValue)
            _settingsPerUserId[id] = settings.Value;
        else
            _settingsPerUserId.Remove(id);
        var settingsToSend = settings ?? AudioPlayerSettings.From(this);
        if (previous == settingsToSend)
            return;
        // TODO: send fake sync vars
    }

}
