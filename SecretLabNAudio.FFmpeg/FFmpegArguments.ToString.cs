using System.Text;
using NorthwoodLib.Pools;
using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg;

public readonly partial record struct FFmpegArguments
{


    public bool IsStandardInput => Input.AsSpan().Trim() is StandardPipe or "pipe:" or "pipe:0";

    public bool IsStandardOutput => Output.AsSpan().Trim() is StandardPipe or "pipe:" or "pipe:1";

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

    public void PrintTo(StringBuilder builder)
    {
        Input.ThrowIfInvalidProcessArgument(InputMissing, InputHasQuotation);
        Output.ThrowIfInvalidProcessArgument("Output must be specified", "Output must not include quotation marks");
        if (!ShowLogs)
            builder.Append(VerbosityError);
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
            builder.Append("-f ").AppendWithTrailingWhitespace(Format);
        if (IsStandardOutput)
            builder.Append(Output);
        else
            builder.Append('"').Append(Output).Append('"');
    }

}
