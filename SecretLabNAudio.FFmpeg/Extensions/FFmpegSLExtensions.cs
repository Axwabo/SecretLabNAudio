// ReSharper disable InvokeAsExtensionMember

namespace SecretLabNAudio.FFmpeg.Extensions;

/// <summary>
/// Extension members for FFmpeg process wrappers.
/// </summary>
public static class FFmpegSLExtensions
{

    extension(FFmpegSL ffmpeg)
    {

        /// <summary>
        /// Starts an FFmpeg process that outputs <see cref="AudioPlayer.SupportedFormat">player-compatible</see> 32-bit floats.
        /// </summary>
        /// <param name="input">The input source (e.g. file path, URL).</param>
        /// <returns>An <see cref="FFmpegSL"/> wrapper if the process was started, null otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="input"/> is null or whitespace, or if it contains a quotation mark.</exception>
        public static FFmpegSL? PlayerCompatibleToStdout(string input)
            => ToStdout(input, AudioPlayer.SampleRate, AudioPlayer.Channels);

        /// <summary>
        /// Starts an FFmpeg process that outputs 32-bit floats.
        /// </summary>
        /// <param name="input">The input source (e.g. file path, URL).</param>
        /// <param name="sampleRate">The sample rate to output.</param>
        /// <param name="channels">The number of channels to output.</param>
        /// <returns>An <see cref="FFmpegSL"/> wrapper if the process was started, null otherwise.</returns>
        /// <exception cref="ArgumentException">Thrown if <paramref name="input"/> is null or whitespace, or if it contains a quotation mark.</exception>
        public static FFmpegSL? ToStdout(string input, int sampleRate, int channels)
            => FFmpegSL.Start(FFmpegArguments.ToStdoutString(input, sampleRate, channels), true);

        /// <summary>
        /// Attempts to send a termination signal to FFmpeg.
        /// </summary>
        /// <param name="timeoutMilliseconds">The amount of millisecond to wait for the process to exit. -1 = indefinite wait.</param>
        /// <returns>True if the process has exited, false otherwise.</returns>
        public bool TryTerminateGracefully(int timeoutMilliseconds = 1000)
        {
            if (ffmpeg.Stdin == null)
                return ffmpeg.HasExited;
            if (ffmpeg.HasExited)
                return true;
            ffmpeg.Stdin.Write('q');
            ffmpeg.Stdin.Flush();
            return timeoutMilliseconds == 0 ? ffmpeg.HasExited : ffmpeg.WaitForExit(timeoutMilliseconds);
        }

    }

    extension(IFFmpegWrapper ffmpeg)
    {

        /// <summary>
        /// Whether the process exited with a non-zero exit code, and has an error message.
        /// </summary>
        public bool HasExitedWithError => ffmpeg is {HasExited: true, ExitCode: not 0} && !string.IsNullOrWhiteSpace(ffmpeg.FinalErrorMessage);

        /// <summary>
        /// Throws an exception if the FFmpeg process has exited with a non-zero exit code, and has an error message.
        /// </summary>
        /// <exception cref="FFmpegRuntimeException">The exception if validation failed.</exception>
        public void ThrowIfExitedWithError()
        {
            if (ffmpeg.HasExitedWithError)
                throw new FFmpegRuntimeException(ffmpeg.FinalErrorMessage!);
        }

    }

}
