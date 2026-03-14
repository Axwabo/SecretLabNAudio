namespace SecretLabNAudio.FFmpeg.Caches;

public abstract record SaveCacheError;

public sealed record InvalidInputError(string? Source) : SaveCacheError;

public sealed record FileNotFoundError(string Path) : SaveCacheError;

public sealed record FFmpegStartupError(NativeErrorCode ErrorCode) : SaveCacheError;

public sealed record FFmpegRuntimeError(string ErrorMessage) : SaveCacheError;
