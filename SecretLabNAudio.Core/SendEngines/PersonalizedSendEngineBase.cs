using VoiceChat.Networking;

namespace SecretLabNAudio.Core.SendEngines;

/// <summary>Base class for personalizing settings on each broadcast per player.</summary>
public abstract class PersonalizedSendEngineBase : SendEngine
{

    /// <summary>The engine that filters targets and broadcasts the audio messages.</summary>
    public SendEngine BaseEngine { get; }

    /// <summary>The <see cref="SpeakerPersonalization"/> this engine operates on.</summary>
    public SpeakerPersonalization Personalization { get; }

    /// <summary>
    /// Creates a new <see cref="PersonalizedSendEngineBase"/>.
    /// </summary>
    /// <param name="baseEngine">The engine that filters targets and broadcasts the audio messages.</param>
    /// <param name="personalization">The <see cref="SpeakerPersonalization"/> this engine operates on.</param>
    protected PersonalizedSendEngineBase(SendEngine baseEngine, SpeakerPersonalization personalization)
    {
        BaseEngine = baseEngine;
        Personalization = personalization;
    }

    /// <inheritdoc/>
    protected internal override bool Broadcast(Player player, AudioMessage message)
    {
        if (!BaseEngine.Broadcast(player, message))
            return false;
        Personalize(player);
        return true;
    }

    /// <summary>Personalizes the settings after the audio message has been sent.</summary>
    /// <param name="player">The player to personalize settings for.</param>
    protected abstract void Personalize(Player player);

}
