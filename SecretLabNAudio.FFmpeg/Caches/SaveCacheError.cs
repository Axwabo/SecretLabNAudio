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

    public static implicit operator SaveCacheError(NativeErrorCode errorCode) => new FFmpegStartupError(errorCode);

    public static implicit operator SaveCacheError(Exception exception) => new ExceptionError(exception);

}

public sealed record InvalidInputError(string? Source) : SaveCacheError;

public sealed record FileNotFoundError(string Path) : SaveCacheError;

public sealed record FFmpegStartupError(NativeErrorCode ErrorCode) : SaveCacheError;

public sealed record FFmpegRuntimeError(string ErrorMessage) : SaveCacheError;

public sealed record CanceledError : SaveCacheError;

public sealed record ExceptionError(Exception Exception) : SaveCacheError;
