namespace SecretLabNAudio.FFmpeg.Interop;

/// <summary>
/// An exception representing a known FFmpeg startup error.
/// </summary>
public sealed class FFmpegStartupException : Exception
{

    /// <summary>
    /// Creates an <see cref="FFmpegStartupException"/> corresponding to the last caught start error. 
    /// </summary>
    public static FFmpegStartupException Last => new(FFmpegSL.LastCaughtStartError);

    /// <summary>
    /// The <see cref="System.ComponentModel.Win32Exception.NativeErrorCode"/> that was caught while starting the FFmpeg process.
    /// </summary>
    public NativeErrorCode Error { get; }

    /// <summary>
    /// Creates a new <see cref="FFmpegStartupException"/> instance.
    /// </summary>
    /// <param name="error">The error that was encountered during startup.</param>
    public FFmpegStartupException(NativeErrorCode error) => Error = error;

}
