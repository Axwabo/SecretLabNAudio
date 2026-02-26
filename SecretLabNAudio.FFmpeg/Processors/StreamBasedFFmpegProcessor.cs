using System.Threading.Tasks;
using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Processors;

using StreamResolver = Func<CancellationToken, Task<Stream>>;

public sealed partial class StreamBasedFFmpegProcessor : AsyncFFmpegProcessorBase
{

    private StreamBasedFFmpegProcessor(StreamResolver resolver, bool isOwned, double capacity, FFmpegArguments arguments)
        : base(capacity, arguments)
        => _ = StartAsync(resolver, isOwned, arguments);

    private async Awaitable StartAsync(StreamResolver resolver, bool isOwned, FFmpegArguments arguments)
    {
        await Awaitable.BackgroundThreadAsync();
        Stream? stream = null;
        try
        {
            stream = await resolver(Token);
            if (!TryStartFFmpeg(arguments, out var ffmpeg))
                return;
            Run(() => BufferLoop(ffmpeg));
            await stream.CopyToAsync(ffmpeg.Stdin!.BaseStream, Token);
        }
        catch (Exception e) when (!Token.IsCancellationRequested)
        {
            AsyncException = e;
        }
        finally
        {
            if (isOwned && stream != null)
                await stream.DisposeAsync();
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
