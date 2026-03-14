using SecretLabNAudio.FFmpeg.Caches;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static class CacheErrorExtensions
{

    extension(SaveCacheError error)
    {

        public string ToHumanReadableString() => error switch
        {
            FileNotFoundError fileNotFound => $"The specified file was not found. File: {fileNotFound.Path}",
            FFmpegStartupError startup => $"FFmpeg failed to start: {startup.ErrorCode switch
            {
                NativeErrorCode.ProcessStartNull => "Process.Start returned null.",
                NativeErrorCode.FileNotFound or NativeErrorCode.PathNotFound => "FFmpeg was not found.",
                NativeErrorCode.AccessDenied => "Access to the executable was denied.",
                _ => $"native error code {startup.ErrorCode} (0x{(int) startup.ErrorCode:x8})"
            }}",
            FFmpegRuntimeError runtime => $"FFmpeg encountered an error: {runtime.ErrorMessage}",
            InvalidInputError invalidInput => $"The specified input source was invalid. Input: {invalidInput.Source}",
            CanceledError => "The operation was canceled.",
            ExceptionError exception => exception.Exception.ToString(),
            _ => error.ToString()
        };

    }

}
