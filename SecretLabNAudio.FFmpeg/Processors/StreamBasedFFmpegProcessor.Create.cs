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

    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(Stream stream, double capacity = DefaultCapacity, bool isOwned = true)
        => CreatePlayerCompatible(Task.FromResult(stream), capacity, isOwned);

    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(StreamResolver resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => new(resolver, isOwned, capacity, PlayerCompatibleArguments with {InputOptions = arguments.InputOptions, OutputOptions = arguments.OutputOptions});

    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(Task<Stream> resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => CreatePlayerCompatible(_ => resolver, arguments, capacity, isOwned);

    public static StreamBasedFFmpegAudioProcessor CreatePlayerCompatible(Stream stream, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => CreatePlayerCompatible(Task.FromResult(stream), arguments, capacity, isOwned);

    public static StreamBasedFFmpegAudioProcessor Create(StreamResolver resolver, int sampleRate, int channels, double capacity = DefaultCapacity, bool isOwned = true)
        => new(resolver, isOwned, capacity, FFmpegArguments.StdinToStdout(sampleRate, channels));

    public static StreamBasedFFmpegAudioProcessor Create(Task<Stream> resolver, int sampleRate, int channels, double capacity = DefaultCapacity, bool isOwned = true)
        => Create(_ => resolver, sampleRate, channels, capacity, isOwned);

    public static StreamBasedFFmpegAudioProcessor Create(Stream stream, int sampleRate, int channels, double capacity = DefaultCapacity, bool isOwned = true)
        => Create(Task.FromResult(stream), sampleRate, channels, capacity, isOwned);

    public static StreamBasedFFmpegAudioProcessor Create(StreamResolver resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => new(resolver, isOwned, capacity, arguments.ReadFromStandardInput().ForFloatPiping());

    public static StreamBasedFFmpegAudioProcessor Create(Task<Stream> resolver, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => Create(_ => resolver, arguments, capacity, isOwned);

    public static StreamBasedFFmpegAudioProcessor Create(Stream stream, FFmpegArguments arguments, double capacity = DefaultCapacity, bool isOwned = true)
        => Create(Task.FromResult(stream), arguments, capacity, isOwned);

}
