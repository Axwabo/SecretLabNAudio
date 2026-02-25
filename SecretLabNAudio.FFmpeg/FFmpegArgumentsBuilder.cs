using SecretLabNAudio.Core;

namespace SecretLabNAudio.FFmpeg;

public record struct FFmpegArgumentsBuilder(string? Input, int SampleRate, int Channels, string? Format, string? Output = "-")
{

    public static FFmpegArgumentsBuilder PlayerCompatible { get; } = new(null, AudioPlayer.SampleRate, AudioPlayer.Channels, "f32le");

    public FFmpegArgumentsBuilder WithStandardInputPipe() => WithInput("-");

    public FFmpegArgumentsBuilder WithStandardOutputPipe() => WithOutput("-");

    public FFmpegArgumentsBuilder WithInput(string input)
    {
        Input = input;
        return this;
    }

    public FFmpegArgumentsBuilder WithOutput(string output)
    {
        Output = output;
        return this;
    }

    public string Build()
    {
        var input = Input ?? throw new InvalidOperationException("Input must be specified");
        var format = Format ?? throw new InvalidOperationException("Format must be specified");
        var output = Output ?? throw new InvalidOperationException("Output must be specified");
        return $"-v error -i \"{input}\" -ar {SampleRate} -ac {Channels} -f {format} \"{output}\"";
    }

    public static implicit operator string(FFmpegArgumentsBuilder builder) => builder.Build();

}
