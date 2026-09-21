using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Outputs;
using SecretLabNAudio.Core.Pools;

namespace SecretLabNAudio.Core;

public abstract class AudioPlayerBase : MonoBehaviour
{

    /// <summary>
    /// The <see cref="SendFilter"/> specifying which players should receive audio packets.
    /// </summary>
    public SendFilter SendFilter { get; set; } = SendFilter.Default;

    /// <summary>
    /// The <see cref="AudioPacketOutput"/> 
    /// </summary>
    public AudioPacketOutput? Output { get; set; }

    public event Action? Ended;

    /// <summary>Invoked when this player is disabled or destroyed.</summary>
    public event Action? Destroyed;

    protected virtual void OnDisable()
    {
        Destroy(this);
        Destroyed.InvokeSafely();
        Destroyed = null;
        Ended = null;
        SendFilter = null!;
        Output = null;
    }

    protected void InvokeEnded()
    {
        Ended.InvokeSafely();
        if (Output is SpeakerToyOutput {Speaker: var speaker, PoolOnEnd: true})
            SpeakerToyPool.Return(speaker);
    }

}
