using System.Threading.Tasks;

namespace SecretLabNAudio.FFmpeg.Processors;

using StreamResolver = Func<CancellationToken, Task<Stream>>;

public sealed partial class StreamBasedFFmpegAudioProcessor
{

    private static readonly FFmpegArguments PlayerCompatibleArguments = FFmpegArguments.PlayerCompatibleStdout with {Input = FFmpegArguments.StandardPipe};

    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(StreamResolver resolver, double capacity = DefaultCapacity, bool isOwned = true)
        => new(resolver, isOwned, capacity, PlayerCompatibleArguments);

    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(Task<Stream> resolver, double capacity = DefaultCapacity, bool isOwned = true)
        => CreatePlayerCompatible(_ => resolver, capacity, isOwned);

    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(StreamResolver resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => new(resolver, isOwned, capacity, PlayerCompatibleArguments with {InputOptions = arguments.InputOptions, OutputOptions = arguments.OutputOptions});

    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(Task<Stream> resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => CreatePlayerCompatible(_ => resolver, arguments, capacity, isOwned);

    public StreamBasedFFmpegAudioProcessor(StreamResolver resolver, int sampleRate, int channels, double capacity = DefaultCapacity, bool isOwned = true)
        : this(resolver, isOwned, capacity, FFmpegArguments.StdinToStdout(sampleRate, channels))
    {
    }

    public StreamBasedFFmpegAudioProcessor(Task<Stream> resolver, int sampleRate, int channels, double capacity = DefaultCapacity, bool isOwned = true)
        : this(_ => resolver, sampleRate, channels, capacity, isOwned)
    {
    }

    public StreamBasedFFmpegAudioProcessor(StreamResolver resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        : this(resolver, isOwned, capacity, arguments.ReadFromStandardInput().ForFloatPiping())
    {
    }

    public StreamBasedFFmpegAudioProcessor(Task<Stream> resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        : this(_ => resolver, arguments, capacity, isOwned)
    {
    }

}
