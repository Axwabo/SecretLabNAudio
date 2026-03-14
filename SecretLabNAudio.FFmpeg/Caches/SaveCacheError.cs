namespace SecretLabNAudio.FFmpeg.Caches;

public abstract record SaveCacheError
{

    public static SaveCacheError Canceled { get; } = new CanceledError();

    public static implicit operator SaveCacheError(Exception exception) => new ExceptionError(exception);

}

public sealed record InvalidInputError(string? Source) : SaveCacheError;

public sealed record FileNotFoundError(string Path) : SaveCacheError;

public sealed record FFmpegStartupError(NativeErrorCode ErrorCode) : SaveCacheError;

public sealed record FFmpegRuntimeError(string ErrorMessage) : SaveCacheError;

public sealed record CanceledError : SaveCacheError;

public sealed record ExceptionError(Exception Exception) : SaveCacheError;
