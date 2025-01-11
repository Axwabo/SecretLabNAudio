using System.Collections.Generic;

namespace SecretLabNAudio.Core;

public class PersonalizedAudioPlayer : TargetedAudioPlayer
{

    private readonly Dictionary<string, AudioPlayerSettings> _settingsPerUserId = [];

    public void Override(ReferenceHub hub, AudioPlayerSettings settings)
    {
        _settingsPerUserId[hub.authManager.UserId] = settings;
        Sync(hub);
    }

    public void Modify(ReferenceHub hub, Func<AudioPlayerSettings, AudioPlayerSettings> settingsTransform, Func<AudioPlayerSettings>? defaultSupplier = null)
    {
        // TODO
    }

    public void ClearOverride(ReferenceHub hub)
    {
        if (_settingsPerUserId.Remove(hub.authManager.UserId))
            Sync(hub);
    }

    private void Sync(ReferenceHub hub)
    {
        // TODO: send fake sync vars
    }

}
