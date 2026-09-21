using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Outputs;

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

    public SpeakerToy? Speaker => (Output as SpeakerToyOutput)?.Speaker;

    /// <summary>Invoked when this player is disabled or destroyed.</summary>
    public event Action? Destroyed;

    protected virtual void OnDisable()
    {
        Destroy(this);
        Destroyed.InvokeSafely();
        Destroyed = null;
        SendFilter = null!;
        Output = null;
    }

}
