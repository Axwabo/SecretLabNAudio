using SecretLabNAudio.Core;

namespace SecretLabNAudio.FFmpeg;

public readonly partial record struct FFmpegArguments
{

    public FFmpegArguments ToPlayerCompatible() => this with
    {
        SampleRate = AudioPlayer.SampleRate,
        Channels = AudioPlayer.Channels
    };

    public FFmpegArguments ForFloatStreaming() => this with
    {
        ShowLogs = false,
        Format = Float32Format,
        Output = StandardPipe
    };

    public FFmpegArguments ForPlayerCompatibleFloatStreaming() => new(false, InputOptions, Input, AudioPlayer.SampleRate, AudioPlayer.Channels, OutputOptions, Float32Format, StandardPipe);

    public FFmpegArguments FromStandardInput() => this with {Input = StandardPipe};

    public FFmpegArguments ToStandardOutput() => this with {Output = StandardPipe};

    public FFmpegArguments WithSampleRate(int sampleRate) => this with {SampleRate = sampleRate};

    public FFmpegArguments WithChannels(int channels) => this with {Channels = channels};

    public FFmpegArguments WithVolumeScalar(double scalar) => this with {OutputOptions = $"-af volume={scalar}"};

    public FFmpegArguments WithVolumeDecibels(double relativeDecibels) => this with {OutputOptions = $"-af volume={relativeDecibels}dB"};

    public FFmpegArguments WithComplexFilter(string filter) => this with {OutputOptions = $"-filter_complex \"{filter}\""};

}
