using SecretLabNAudio.Core.Builders;

namespace SecretLabNAudio.Core.Outputs;

public class SpeakerToyOutput : AudioPacketOutput, ISpeakerBuilder
{

    public SpeakerToy Speaker { get; }

    public bool PoolOnEnd { get; set; }

    public SpeakerToyOutput(SpeakerToy speaker) => Speaker = speaker;

    public override void BroadcastEncodedData(byte[] buffer, int length, SendFilter filter)
        => filter.Broadcast(new AudioMessage(Speaker.ControllerId, buffer, length));

    SpeakerToyOutput ISpeakerBuilder.Output => this;

}
