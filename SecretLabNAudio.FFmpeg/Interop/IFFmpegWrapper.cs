namespace SecretLabNAudio.FFmpeg.Interop;

/// <summary>
/// A base interface for objects wrapping an FFmpeg process.
/// </summary>
public interface IFFmpegWrapper
{

    /// <summary>
    /// Gets a value indicating whether the associated process has been terminated.
    /// The value may be false if asynchronous pipe handlers (e.g. the standard output) have not yet completed processing.
    /// </summary>
    /// <exception cref="InvalidOperationException">There is no process associated with the object.</exception>
    /// <exception cref="System.ComponentModel.Win32Exception">The exit code for the process could not be retrieved.</exception>
    /// <seealso cref="ExitCode"/>
    bool HasExited { get; }

    /// <summary>
    /// Gets the value that the associated process specified when it terminated.
    /// </summary>
    /// <exception cref="InvalidOperationException">The process has not yet exited or the process handle is not valid.</exception>
    int ExitCode { get; }

    /// <summary>
    /// Returns the cached output of the standard error.
    /// If the cached value is null, reads the standard error to the end if the process has exited, and caches the result.
    /// If the value isn't cached, and the process has not yet exited or has been disposed, returns null.
    /// </summary>
    /// <remarks>
    /// The returned value (if not null) ends with a newline.
    /// Call <see cref="string.IsNullOrWhiteSpace"/> to check whether there's an error message.
    /// </remarks>
    string? FinalErrorMessage { get; }

    /// <summary>
    /// Whether the underlying process instance has been disposed.
    /// </summary>
    bool IsDisposed { get; }

}
