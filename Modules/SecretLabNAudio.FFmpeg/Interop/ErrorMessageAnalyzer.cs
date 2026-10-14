namespace SecretLabNAudio.FFmpeg.Interop;

internal static class ErrorMessageAnalyzer
{

    public static bool HasNoOutputStream(string? message)
    {
        var span = message.AsSpan();
        var index = span.IndexOfAny('\n', '\r');
        if (index == -1)
            return false;
        var line = span[..index];
        return line.StartsWith("[out#0/f32le") && line.EndsWith("Output file does not contain any stream");
    }

}
