using VoiceChat.Networking;

namespace SecretLabNAudio.Core.SendEngines;

public delegate AudioPlayerSettings? PersonalizedSettingsTransform(ReferenceHub hub, AudioPlayerSettings? current);

public class LivePersonalizedSendEngine : SendEngine
{

    private readonly SendEngine _baseEngine;
    private readonly AudioPlayerPersonalization _personalization;
    private readonly PersonalizedSettingsTransform _settingsTransform;

    public LivePersonalizedSendEngine(SendEngine baseEngine, AudioPlayerPersonalization personalization, PersonalizedSettingsTransform settingsTransform)
    {
        _baseEngine = baseEngine;
        _personalization = personalization;
        _settingsTransform = settingsTransform;
    }

    protected internal override bool Broadcast(ReferenceHub hub, AudioMessage message)
    {
        if (!_baseEngine.Broadcast(hub, message))
            return false;
        _personalization.Modify(hub, settings => _settingsTransform(hub, settings));
        return true;
    }

}
