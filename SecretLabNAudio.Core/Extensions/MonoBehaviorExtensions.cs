namespace SecretLabNAudio.Core.Extensions;

/// <summary>Extension methods for <see cref="MonoBehaviour"/> components.</summary>
public static class MonoBehaviorExtensions
{

    /// <summary>Gets the <see cref="SpeakerToy"/> wrapper from the given component.</summary>
    /// <param name="behavior">The <see cref="MonoBehaviour"/> to get the speaker from.</param>
    /// <param name="exceptionMessage">The message to construct the exceptionm with if the component is missing.</param>
    /// <returns>The wrapper of the speaker on the <paramref name="behavior"/>'s game object.</returns>
    /// <exception cref="MissingComponentException">Thrown if the <paramref name="behavior"/> does not have a <see cref="SpeakerToy"/> component.</exception>
    public static SpeakerToy GetSpeaker(this MonoBehaviour behavior, string exceptionMessage)
    {
        if (!behavior.TryGetComponent(out AdminToys.SpeakerToy toy))
            throw new MissingComponentException(exceptionMessage);
        return SpeakerToy.Get(toy);
    }

}
