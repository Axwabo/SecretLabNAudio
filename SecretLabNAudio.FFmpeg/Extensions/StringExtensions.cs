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

        public void ValidateProcessArgument(string emptyMessage, string quotationMessage)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new InvalidOperationException(emptyMessage);
            if (s!.Contains('"'))
                throw new InvalidOperationException(quotationMessage);
        }

        public string ValidateProcessArgument(string paramName, string emptyMessage, string quotationMessage)
            => string.IsNullOrWhiteSpace(s)
                ? throw new ArgumentException(emptyMessage, paramName)
                : s!.Contains('"')
                    ? throw new ArgumentException(quotationMessage, paramName)
                    : s;

        [return: NotNullIfNotNull(nameof(s))]
        public string? ValidateNoQuotation(string paramName, string message)
        {
            if (s != null && s.Contains('"'))
                throw new ArgumentException(message, paramName);
            return s;
        }

    }

}
