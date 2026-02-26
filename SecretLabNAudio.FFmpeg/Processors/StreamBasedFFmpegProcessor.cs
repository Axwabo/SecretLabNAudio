using System.Threading.Tasks;
using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Processors;

using StreamResolver = Func<CancellationToken, Task<Stream>>;

public sealed class StreamBasedFFmpegProcessor : AsyncFFmpegProcessorBase
{

    public static StreamBasedFFmpegProcessor CreatePlayerCompatible(StreamResolver resolver, double capacity = 10, bool isOwned = true)
        => new(resolver, capacity, AudioPlayer.SupportedFormat, isOwned);

    public static StreamBasedFFmpegProcessor CreatePlayerCompatible(Task<Stream> resolver, double capacity = 10, bool isOwned = true)
        => CreatePlayerCompatible(_ => resolver, capacity, isOwned);

    public StreamBasedFFmpegProcessor(Task<Stream> resolver, int sampleRate, int channels, double capacity = 10, bool isOwned = true) : this(_ => resolver, sampleRate, channels, capacity, isOwned)
    {
    }

    public StreamBasedFFmpegProcessor(StreamResolver resolver, int sampleRate, int channels, double capacity = 10, bool isOwned = true)
        : this(resolver, capacity, WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels), isOwned)
    {
    }

    private StreamBasedFFmpegProcessor(StreamResolver resolver, double capacity, WaveFormat format, bool isOwned)
        : base(capacity, format)
        => _ = StartAsync(resolver, isOwned);

    private async Awaitable StartAsync(StreamResolver resolver, bool isOwned)
    {
        await Awaitable.BackgroundThreadAsync();
        Stream? stream = null;
        try
        {
            stream = await resolver(Token);
            if (!TryStartFFmpeg(FFmpegArguments.StandardPipe, out var ffmpeg))
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
