using System.Threading.Tasks;

namespace SecretLabNAudio.FFmpeg.Processors;

using StreamResolver = Func<CancellationToken, Task<Stream>>;

public sealed partial class StreamBasedFFmpegAudioProcessor : AsyncFFmpegProcessorBase
{

    private StreamBasedFFmpegAudioProcessor(StreamResolver resolver, bool isOwned, double capacity, FFmpegArguments transformedArguments)
        : base(capacity, transformedArguments)
    {
        var arguments = transformedArguments.ToArgumentsString();
        _ = StartAsync(resolver, isOwned, arguments);
    }

    private async Awaitable StartAsync(StreamResolver resolver, bool isOwned, string arguments)
    {
        BufferingState = AsyncBufferingState.ResolvingStream;
        await Awaitable.BackgroundThreadAsync();
        Stream? stream = null;
        Stream? standardInput = null;
        try
        {
            stream = await resolver(Token).ConfigureAwait(false);
            BufferingState = AsyncBufferingState.StartingFFmpeg;
            if (!TryStartFFmpeg(arguments, out var ffmpeg))
                return;
            Offload(() => BufferLoop(ffmpeg));
            await stream.CopyToAsync(standardInput = ffmpeg.Stdin.BaseStream, Token).ConfigureAwait(false);
        }
        catch (Exception e) when (!Token.IsCancellationRequested)
        {
            AsyncException = e;
        }
        finally
        {
            if (isOwned && stream != null)
                await stream.DisposeAsync().ConfigureAwait(false);
            standardInput?.Close();
        }
    }

}
