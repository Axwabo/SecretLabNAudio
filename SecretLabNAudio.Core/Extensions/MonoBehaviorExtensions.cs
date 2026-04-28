namespace SecretLabNAudio.Core.Extensions;

internal static class MonoBehaviorExtensions
{

    public static SpeakerToy GetSpeaker(this MonoBehaviour behavior, string exceptionMessage)
        => behavior.TryGetComponent(out AdminToys.SpeakerToy toy)
            ? SpeakerToy.Get(toy)
            : throw new MissingComponentException(exceptionMessage);

    public static void InvokeSafely(this Action? @event)
    {
        try
        {
            @event?.Invoke();
        }
        catch (Exception e)
        {
#if DEBUG
            Console.WriteLine(e);
#else
            Debug.LogError(e);
#endif
        }
    }

}
