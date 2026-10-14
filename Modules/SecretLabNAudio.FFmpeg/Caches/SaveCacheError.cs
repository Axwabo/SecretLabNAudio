namespace SecretLabNAudio.FFmpeg.Caches;

/// <summary>
/// Represents error states that can occur while saving an audio file to a cache.
/// </summary>
public abstract record SaveCacheError
{

    /// <summary>
    /// The error returned when the <see cref="CancellationToken"/> signals cancellation.
    /// </summary>
    public static SaveCacheError Canceled { get; } = new CanceledError();

    /// <summary>
    /// Creates a <see cref="FFmpegStartupError"/> based on the error code.
    /// </summary>
    /// <param name="errorCode">The error code to convert.</param>
    /// <returns>A new <see cref="FFmpegStartupError"/> as a <see cref="SaveCacheError"/>.</returns>
    public static implicit operator SaveCacheError(NativeErrorCode errorCode) => new FFmpegStartupError(errorCode);

    /// <summary>
    /// Creates an <see cref="ExceptionError"/> from the exception.
    /// </summary>
    /// <param name="exception">The exception to encapsulate.</param>
    /// <returns>A new <see cref="ExceptionError"/> as a <see cref="SaveCacheError"/>.</returns>
    public static implicit operator SaveCacheError(Exception exception) => new ExceptionError(exception);

}

/// <summary>
/// The error representing an invalid input argument.
/// </summary>
/// <param name="Source">The source that was provided.</param>
public sealed record InvalidInputError(string? Source) : SaveCacheError;

/// <summary>
/// The error representing a file that was not found.
/// </summary>
/// <param name="Path">The provided file path.</param>
public sealed record FileNotFoundError(string Path) : SaveCacheError;

/// <summary>
/// The error representing an FFmpeg startup failure.
/// </summary>
/// <param name="ErrorCode">The <see cref="System.ComponentModel.Win32Exception.NativeErrorCode"/> that was caught while starting FFmpeg.</param>
public sealed record FFmpegStartupError(NativeErrorCode ErrorCode) : SaveCacheError;

/// <summary>
/// The error representing an error outputted by FFmpeg during its runtime.
/// </summary>
/// <param name="ErrorMessage">The full output of the standard error.</param>
public sealed record FFmpegRuntimeError(string ErrorMessage) : SaveCacheError;

/// <summary>
/// The error representing the cancellation signaled by a <see cref="CancellationToken"/>.
/// </summary>
public sealed record CanceledError : SaveCacheError;

/// <summary>
/// The error encapsulating an <see cref="System.Exception"/>.
/// </summary>
/// <param name="Exception">The exception that was caught.</param>
public sealed record ExceptionError(Exception Exception) : SaveCacheError;
