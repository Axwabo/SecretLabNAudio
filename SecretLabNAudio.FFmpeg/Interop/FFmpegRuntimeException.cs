namespace SecretLabNAudio.FFmpeg.Interop;

/// <summary>
/// An exception containing an error reported by an FFmpeg process.
/// </summary>
public sealed class FFmpegRuntimeException : Exception
{

    /// <summary>
    /// Creates a new <see cref="FFmpegRuntimeException"/> instance.
    /// </summary>
    /// <param name="message">The error logged by FFmpeg.</param>
    public FFmpegRuntimeException(string message) : base(message)
    {
    }

}
