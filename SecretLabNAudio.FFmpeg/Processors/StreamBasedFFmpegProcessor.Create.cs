using System.Threading.Tasks;

namespace SecretLabNAudio.FFmpeg.Processors;

using StreamResolver = Func<CancellationToken, Task<Stream>>;

public sealed partial class StreamBasedFFmpegProcessor
{

    private static readonly FFmpegArguments PlayerCompatibleArguments = FFmpegArguments.PlayerCompatibleStdout with {Input = FFmpegArguments.StandardPipe};

    public static StreamBasedFFmpegProcessor CreatePlayerCompatible(StreamResolver resolver, double capacity = DefaultCapacity, bool isOwned = true)
        => new(resolver, isOwned, capacity, PlayerCompatibleArguments);

    public static StreamBasedFFmpegProcessor CreatePlayerCompatible(Task<Stream> resolver, double capacity = DefaultCapacity, bool isOwned = true)
        => CreatePlayerCompatible(_ => resolver, capacity, isOwned);

    public static StreamBasedFFmpegProcessor CreatePlayerCompatible(StreamResolver resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => new(resolver, isOwned, capacity, PlayerCompatibleArguments with {InputOptions = arguments.InputOptions, OutputOptions = arguments.OutputOptions});

    public static StreamBasedFFmpegProcessor CreatePlayerCompatible(Task<Stream> resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => CreatePlayerCompatible(_ => resolver, arguments, capacity, isOwned);

    public StreamBasedFFmpegProcessor(StreamResolver resolver, int sampleRate, int channels, double capacity = DefaultCapacity, bool isOwned = true)
        : this(resolver, isOwned, capacity, FFmpegArguments.StdinToStdout(sampleRate, channels))
    {
    }

    public StreamBasedFFmpegProcessor(Task<Stream> resolver, int sampleRate, int channels, double capacity = DefaultCapacity, bool isOwned = true)
        : this(_ => resolver, sampleRate, channels, capacity, isOwned)
    {
    }

    public StreamBasedFFmpegProcessor(StreamResolver resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        : this(resolver, isOwned, capacity, arguments.ReadFromStandardInput().ForFloatPiping())
    {
    }

    public StreamBasedFFmpegProcessor(Task<Stream> resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        : this(_ => resolver, arguments, capacity, isOwned)
    {
    }

}
