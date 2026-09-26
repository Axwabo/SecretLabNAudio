using System.Threading;

namespace SecretLabNAudio.Core.Encoded;

public class PreEncodedAudioPlayer : AudioPlayerBase
{

    private static readonly byte[] EncodedBuffer = new byte[AudioConstants.MaxEncodedBytesPerPacket];

    public IOpusPacketProvider? PacketProvider
    {
        get;
        set
        {
            field = value;
            _ = ProcessAsync(destroyCancellationToken);
        }
    }

    private async Awaitable ProcessAsync(CancellationToken cancellationToken)
    {
        if (PacketProvider is not { } provider)
            return;
        var time = 0f;
        while (!cancellationToken.IsCancellationRequested)
        {
            await Awaitable.NextFrameAsync(cancellationToken);
            time += Time.deltaTime;
            while (time > 0)
            {
                time -= AudioConstants.PacketDuration;
                var read = provider.ReadPacket(EncodedBuffer);
                if (read == 0)
                {
                    InvokeEnded();
                    return;
                }

                Output?.BroadcastEncodedData(EncodedBuffer, read, SendFilter);
            }
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PacketProvider?.Dispose(); // TODO: ownership
        PacketProvider = null;
    }

}
