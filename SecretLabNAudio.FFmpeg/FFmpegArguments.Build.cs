using SecretLabNAudio.Core;
using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg;

public readonly partial record struct FFmpegArguments
{

    public FFmpegArguments ToPlayerCompatible() => this with
    {
        SampleRate = AudioPlayer.SampleRate,
        Channels = AudioPlayer.Channels
    };

    public FFmpegArguments ForFloatPiping() => this with
    {
        ShowLogs = false,
        MuxerFormat = Float32Format,
        Output = StandardPipe
    };

    public FFmpegArguments ForPlayerCompatibleFloatPiping() => new(false, InputOptions, Input, AudioPlayer.SampleRate, AudioPlayer.Channels, OutputOptions, Float32Format, StandardPipe);

    public FFmpegArguments ReadFromStandardInput() => this with {Input = StandardPipe};

    public FFmpegArguments PipeToStandardOutput() => this with {Output = StandardPipe};

    public FFmpegArguments WithInput(string input) => this with {Input = input.ValidateProcessArgument(nameof(input), InputMissing, InputHasQuotation)};

    public FFmpegArguments WithOutput(string output) => this with {Output = output.ValidateProcessArgument(nameof(output), OutputMissing, OutputHasQuotation)};

    public FFmpegArguments WithSampleRate(int sampleRate) => this with {SampleRate = sampleRate};

    public FFmpegArguments WithChannels(int channels) => this with {Channels = channels};

    public FFmpegArguments WithVolumeScalar(double scalar) => this with {OutputOptions = $"-af volume={scalar}"};

    public FFmpegArguments WithVolumeDecibels(double relativeDecibels) => this with {OutputOptions = $"-af volume={relativeDecibels}dB"};

    public FFmpegArguments WithComplexFilter(string filter) => this with {OutputOptions = $"-filter_complex \"{filter}\""};

    public FFmpegArguments EnableVerboseLogging() => this with {ShowLogs = true};

    public FFmpegArguments WithInputOptions(string? inputOptions) => this with {InputOptions = inputOptions};

    public FFmpegArguments WithOutputOptions(string? outputOptions) => this with {OutputOptions = outputOptions};

    public FFmpegArguments WithMuxerFormat(string? muxerFormat) => this with {MuxerFormat = muxerFormat};
    
    public FFmpegArguments WithInfiniteLoop() => this with {InputOptions = "-stream_loop -1"};

    public FFmpegArguments WithLoopCount(int count) => this with {InputOptions = $"-stream_loop {count}"};

}
