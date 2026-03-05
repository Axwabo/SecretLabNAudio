using System.Text;
using NorthwoodLib.Pools;
using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg;

public readonly partial record struct FFmpegArguments
{

    /// <summary>Whether the <see cref="Input"/> equals the standard input pipe.</summary>
    public bool IsStandardInput => Input.AsSpan().Trim() is StandardPipe or "pipe:" or "pipe:0";

    /// <summary>Whether the <see cref="Output"/> equals the standard output pipe.</summary>
    public bool IsStandardOutput => Output.AsSpan().Trim() is StandardPipe or "pipe:" or "pipe:1";

    /// <summary>
    /// Converts this instance to a string that can be used as <see cref="ProcessStartInfo.Arguments"/>.
    /// </summary>
    /// <returns>The instance as a process arguments string.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if either <see cref="Input"/> or <see cref="Output"/> matches any of the following:
    /// <list type="bullet">
    /// <item><description><see cref="string.IsNullOrWhiteSpace">null or whitespace</see></description></item>
    /// <item><description>contains a quotation mark (<c>&quot;</c>)</description></item>
    /// </list>
    /// </exception>
    public string ToArgumentsString()
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

    /// <summary>
    /// Prints the arguments to a <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="builder">The builder to append to.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if either <see cref="Input"/> or <see cref="Output"/> matches any of the following:
    /// <list type="bullet">
    /// <item><description><see cref="string.IsNullOrWhiteSpace">null or whitespace</see></description></item>
    /// <item><description>contains a quotation mark (<c>&quot;</c>)</description></item>
    /// </list>
    /// </exception>
    // TODO: xml docs file
    public void PrintTo(StringBuilder builder)
    {
        Input.ValidateProcessArgument(InputMissing, InputHasQuotation);
        Output.ValidateProcessArgument(OutputMissing, OutputHasQuotation);
        if (!ShowLogs)
            builder.Append(VerbosityError);
        builder.AppendWithTrailingWhitespace(InputOptions);
        if (IsStandardInput)
            builder.Append($"-i {StandardPipe} ");
        else
            builder.Append("-i \"").Append(Input).Append("\" ");
        if (SampleRate != 0)
            builder.Append("-ar ").Append(SampleRate).Append(' ');
        if (Channels != 0)
            builder.Append("-ac ").Append(Channels).Append(' ');
        builder.AppendWithTrailingWhitespace(OutputOptions);
        if (!string.IsNullOrWhiteSpace(MuxerFormat))
            builder.Append("-f ").AppendWithTrailingWhitespace(MuxerFormat);
        if (IsStandardOutput)
            builder.Append(StandardPipe);
        else
            builder.Append('"').Append(Output).Append('"');
    }

}
