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

    protected virtual void OnDisable()
    {
        Destroy(this);
        SendFilter = null!;
        Output = null;
    }

}
