using System.Text;

namespace SecretLabNAudio.FFmpeg.Extensions;

internal static class StringExtensions
{

    extension(StringBuilder builder)
    {

        public void AppendWithTrailingWhitespace(string? s)
        {
            if (!string.IsNullOrWhiteSpace(s))
                builder.Append(s).Append(' ');
        }

    }

    extension(string? s)
    {

        public void ThrowIfInvalidProcessArgument(string emptyMessage, string quotationMessage)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new InvalidOperationException(emptyMessage);
            if (s!.Contains('"'))
                throw new InvalidOperationException(quotationMessage);
        }

    }

}
