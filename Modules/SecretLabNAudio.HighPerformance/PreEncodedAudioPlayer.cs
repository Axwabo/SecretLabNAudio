using System.Threading;
using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Opus;
using SecretLabNAudio.Core.Processors;

namespace SecretLabNAudio.HighPerformance;

public class PreEncodedAudioPlayer : AudioPlayerBase
{

    private static readonly byte[] EncodedBuffer = new byte[AudioConstants.MaxEncodedBytesPerPacket];

    public IOpusPacketProvider? PacketProvider { get; set; }

    private async Awaitable ProcessAsync(CancellationToken cancellationToken)
    {
        var time = 0f;
        while (!cancellationToken.IsCancellationRequested)
        {
            if (PacketProvider is { } provider)
                SendPackets(provider, ref time);
            await Awaitable.NextFrameAsync(cancellationToken);
        }
    }

    private void SendPackets(IOpusPacketProvider provider, ref float time)
    {
        time += Time.deltaTime;
        while (time > 0)
            ProcessPacket(provider, ref time);
    }

    private void ProcessPacket(IOpusPacketProvider provider, ref float time)
    {
        int read;
        try
        {
            read = provider.ReadPacket(EncodedBuffer);
        }
        catch (Exception e)
        {
            read = 0;
            Debug.LogError(e);
        }

        if (read > 1)
        {
            time -= TocByte.TotalSamples(EncodedBuffer) * AudioConstants.SecondsPerSample;
            Output?.BroadcastEncodedData(EncodedBuffer, read, SendFilter);
            return;
        }

        time = 0;
        End();
    }

    private void End()
    {
        InvokeEnded();
    }

    public override TimeSpan CurrentTime
    {
        get => (PacketProvider as ISeekable)?.CurrentTime ?? TimeSpan.Zero;
        set => (PacketProvider as ISeekable)?.CurrentTime = value;
    }

    public override TimeSpan TotalTime => throw new NotImplementedException();

    protected virtual void Start() => _ = ProcessAsync(destroyCancellationToken);

    protected override void Stop() => PacketProvider = null;

    protected override void OnDisable()
    {
        base.OnDisable();
        PacketProvider?.Dispose(); // TODO: ownership
        PacketProvider = null;
    }

}
