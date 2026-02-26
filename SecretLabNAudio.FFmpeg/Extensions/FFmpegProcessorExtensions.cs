using SecretLabNAudio.FFmpeg.Processors;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static class FFmpegProcessorExtensions
{

    extension(AsyncFFmpegProcessorBase processor)
    {

        public void HandleError(Action<NativeErrorCode> startup, Action<Exception> asyncException, Action<string> ffmpegError, Action success)
        {
            if (processor.StartupError != NativeErrorCode.None)
                startup(processor.StartupError);
            else if (processor.AsyncException != null)
                asyncException(processor.AsyncException);
            else if (!string.IsNullOrWhiteSpace(processor.FinalErrorMessage))
                ffmpegError(processor.FinalErrorMessage!);
            else
                success();
        }

        public T MapError<T>(Func<NativeErrorCode, T> startup, Func<Exception, T> asyncException, Func<string, T> ffmpegError, Func<T> success)
            => processor.StartupError != NativeErrorCode.None
                ? startup(processor.StartupError)
                : processor.AsyncException != null
                    ? asyncException(processor.AsyncException)
                    : !string.IsNullOrWhiteSpace(processor.FinalErrorMessage)
                        ? ffmpegError(processor.FinalErrorMessage!)
                        : success();

    }

}
