using SecretLabNAudio.FFmpeg.Processors;

namespace SecretLabNAudio.FFmpeg.Extensions;

/// <summary>
/// Extension methods for <see cref="AsyncFFmpegProcessorBase"/>s.
/// </summary>
public static class FFmpegProcessorExtensions
{

    extension(AsyncFFmpegProcessorBase processor)
    {

        /// <summary>
        /// Handles the error state based on 4 delegates.
        /// </summary>
        /// <param name="startup">The action to invoke if FFmpeg failed to start.</param>
        /// <param name="asyncException">The action to invoke if an exception occurred in the buffering thread.</param>
        /// <param name="ffmpegError">The action to invoke if FFmpeg outputted an error message.</param>
        /// <param name="success">The action to invoke if no error occurred.</param>
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

        /// <summary>
        /// Maps the error states to a single object.
        /// </summary>
        /// <param name="startup">The result delegate to invoke if FFmpeg failed to start.</param>
        /// <param name="asyncException">The result delegate to invoke if an exception occurred in the buffering thread.</param>
        /// <param name="ffmpegError">The result delegate to invoke if FFmpeg outputted an error message.</param>
        /// <param name="success">The result delegate to invoke if no error occurred.</param>
        /// <typeparam name="T">The type of result to map to.</typeparam>
        /// <returns>The result provided by the adequate delegate.</returns>
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
