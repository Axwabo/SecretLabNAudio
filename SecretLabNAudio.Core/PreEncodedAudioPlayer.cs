namespace SecretLabNAudio.Core;

public class PreEncodedAudioPlayer : AudioPlayerBase
{

    private static readonly byte[] EncoderBuffer = new byte[AudioConstants.MaxEncodedBytesPerPacket];

    public Stream? Source { get; set; }

    private void ProcessPacket()
    {
        if (Source == null)
            return;
        while (Source.Read(EncoderBuffer, 0, 2) >= sizeof(ushort))
        {
            var count = BitConverter.ToUInt16(EncoderBuffer);
            var read = Source.Read(EncoderBuffer, 0, count);
            Output?.BroadcastEncodedData(EncoderBuffer, read, SendFilter);
            if (read >= count)
                continue;
            InvokeEnded();
            break;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Source?.Dispose(); // TODO: ownership
        Source = null;
    }

}
