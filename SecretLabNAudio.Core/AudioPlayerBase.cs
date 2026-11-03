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

    public EndAction EndAction { get; set; }

    /// <summary>Whether the playback is paused.</summary>
    public bool IsPaused { get; set; }

    /// <summary>True if playback has finished on the previous frame.</summary>
    public bool HasEnded { get; protected set; }

    public abstract TimeSpan CurrentTime { get; set; }

    public abstract TimeSpan TotalTime { get; }

    public event Action? Ended;

    /// <summary>Invoked when this player is disabled or destroyed.</summary>
    public event Action? Destroyed;

    protected internal abstract void Stop();

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
        if (HasEnded)
            return;
        HasEnded = true;
        Ended.InvokeSafely();
        switch (EndAction)
        {
            case EndAction.Pool:
                if (this.TryGetSpeaker(out var speaker))
                    SpeakerToyPool.Return(speaker);
                Destroy(this);
                break;
            case EndAction.Destroy:
                this.OutputSpeaker?.DestroySafe();
                Destroy(this);
                break;
            case EndAction.Stop:
                Stop();
                break;
            case EndAction.Nothing:
                break;
        }
    }

}
