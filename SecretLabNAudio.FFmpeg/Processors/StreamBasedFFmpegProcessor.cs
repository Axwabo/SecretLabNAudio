using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Processors;

public sealed class StreamBasedFFmpegProcessor : AsyncFFmpegProcessorBase
{

    private StreamBasedFFmpegProcessor(Func<Awaitable<Stream>> resolver, double capacity, WaveFormat format) : base(capacity, format) => _ = StartAsync(resolver);

    private async Awaitable StartAsync(Func<Awaitable<Stream>> resolver)
    {
        await Awaitable.BackgroundThreadAsync();
        try
        {
            await using var stream = await resolver();
            if (TryStartFFmpeg(FFmpegArguments.StandardPipe, out var ffmpeg))
                await stream.CopyToAsync(ffmpeg.Stdin!.BaseStream, Token);
        }
        catch (Exception e) when (!Token.IsCancellationRequested)
        {
            AsyncException = e;
        }
    }

    /// <inheritdoc />
    public override void StopBuffering()
    {
        base.StopBuffering();
        if (!Disposed && Process is {HasExited: false})
            Process.TryTerminateGracefully(0);
    }

}
