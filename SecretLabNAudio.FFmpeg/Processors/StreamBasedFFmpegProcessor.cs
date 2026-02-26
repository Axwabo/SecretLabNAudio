using System.Threading.Tasks;
using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Processors;

using StreamResolver = Func<CancellationToken, Task<Stream>>;

public sealed partial class StreamBasedFFmpegProcessor : AsyncFFmpegProcessorBase
{

    private StreamBasedFFmpegProcessor(StreamResolver resolver, bool isOwned, double capacity, FFmpegArguments transformedArguments)
        : base(capacity, transformedArguments)
    {
        var arguments = transformedArguments.ToString();
        _ = StartAsync(resolver, isOwned, arguments);
    }

    private async Awaitable StartAsync(StreamResolver resolver, bool isOwned, string arguments)
    {
        BufferingState = AsyncBufferingState.ResolvingStream;
        await Awaitable.BackgroundThreadAsync();
        Stream? stream = null;
        try
        {
            stream = await resolver(Token);
            BufferingState = AsyncBufferingState.StartingFFmpeg;
            if (!TryStartFFmpeg(arguments, out var ffmpeg))
                return;
            Offload(() => BufferLoop(ffmpeg));
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
