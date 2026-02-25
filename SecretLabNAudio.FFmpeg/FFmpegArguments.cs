using System.Text;
using NorthwoodLib.Pools;
using SecretLabNAudio.Core;
using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg;

public record struct FFmpegArguments(bool ShowLogs, string? InputOptions, string? Input, int SampleRate, int Channels, string? OutputOptions, string? Format, string? Output)
{

    public const string StandardPipe = "-";

    public const string Float32 = "f32le";

    public static FFmpegArguments PlayerCompatibleTemplate { get; } = new()
    {
        SampleRate = AudioPlayer.SampleRate,
        Channels = AudioPlayer.Channels,
        Format = Float32,
        Output = StandardPipe
    };

    public readonly bool IsStandardInput => Input.AsSpan().Trim() is StandardPipe or "pipe:" or "pipe:0";

    public readonly bool IsStandardOutput => Output.AsSpan().Trim() is StandardPipe or "pipe:" or "pipe:1";

    public override string ToString()
    {
        var sb = StringBuilderPool.Shared.Rent();
        try
        {
            PrintTo(sb);
            return sb.ToString();
        }
        finally
        {
            StringBuilderPool.Shared.Return(sb);
        }
    }

    public readonly void PrintTo(StringBuilder builder)
    {
        Input.ThrowIfInvalidProcessArgument("Input must be specified", "Input must not include quotation marks");
        Output.ThrowIfInvalidProcessArgument("Output must be specified", "Output must not include quotation marks");
        if (!ShowLogs)
            builder.Append("-v error ");
        builder.AppendWithTrailingWhitespace(InputOptions);
        if (IsStandardInput)
            builder.Append("-i - ");
        else
            builder.Append("-i \"").Append(Input).Append("\" ");
        if (SampleRate != 0)
            builder.Append("-ar ").Append(SampleRate).Append(' ');
        if (Channels != 0)
            builder.Append("-ac ").Append(Channels).Append(' ');
        builder.AppendWithTrailingWhitespace(OutputOptions);
        if (!string.IsNullOrWhiteSpace(Format))
            builder.Append("-f \"").Append(Format).Append("\" ");
        if (IsStandardOutput)
            builder.Append(Output);
        else
            builder.Append('"').Append(Output).Append('"');
        if (builder[^1] == ' ')
            builder.Remove(builder.Length - 1, 1);
    }

}
