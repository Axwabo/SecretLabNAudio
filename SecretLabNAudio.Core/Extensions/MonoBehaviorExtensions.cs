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
            Debug.LogError(e);
        }
    }

    public static void InvokeSafely<T1, T2>(this Action<T1, T2>? @event, T1 arg1, T2 arg2)
    {
        try
        {
            @event?.Invoke(arg1, arg2);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

}
