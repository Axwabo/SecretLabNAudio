using SecretLabNAudio.FFmpeg.Caches;

namespace SecretLabNAudio.FFmpeg.Extensions;

/// <summary>
/// Extension members for cache errors.
/// </summary>
public static class CacheErrorExtensions
{

    /// <param name="error">The error to process.</param>
    extension(SaveCacheError error)
    {

        /// <summary>
        /// Converts the error into a human-readable format.
        /// </summary>
        /// <returns>A human-readable string.</returns>
        public string ToHumanReadableString() => error switch
        {
            FileNotFoundError {Path: var path} => $"The specified file was not found. File: {path}",
            FFmpegStartupError {ErrorCode: var code} => $"FFmpeg failed to start: {code switch
            {
                NativeErrorCode.ProcessStartNull => "Process.Start returned null.",
                NativeErrorCode.FileNotFound or NativeErrorCode.PathNotFound => "FFmpeg was not found.",
                NativeErrorCode.AccessDenied => "Access to the executable was denied.",
                _ => $"native error code {code} (0x{(int) code:x8})"
            }}",
            FFmpegRuntimeError {ErrorMessage: var message} => $"FFmpeg encountered an error: {message}",
            InvalidInputError {Source: var source} => $"The specified input source was invalid. Input: {source}",
            CanceledError => "The operation was canceled.",
            ExceptionError {Exception.Message: var message} => message,
            _ => error.ToString()
        };

    }

}
