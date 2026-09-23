namespace SecretLabNAudio.Core.Encoded;

public class PreEncodedAudioPlayer : AudioPlayerBase
{

    private static readonly byte[] EncodedBuffer = new byte[AudioConstants.MaxEncodedBytesPerPacket];

    public IOpusPacketProvider? PacketProvider { get; set; }

    private void ProcessPacket()
    {
        if (PacketProvider == null)
            return;
        int read;
        while ((read = PacketProvider.ReadPacket(EncodedBuffer)) > 0)
            Output?.BroadcastEncodedData(EncodedBuffer, read, SendFilter);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PacketProvider?.Dispose(); // TODO: ownership
        PacketProvider = null;
    }

}
