using SecretLabNAudio.Core;
using SecretLabNAudio.FFmpeg.Interop;

// ReSharper disable InvokeAsExtensionMember

namespace SecretLabNAudio.FFmpeg.Extensions;

public static class FFmpegSLExtensions
{

    extension(FFmpegSL ffmpeg)
    {

        public static FFmpegSL? PlayerCompatibleToStdout(string input)
            => ToStdout(input, AudioPlayer.SampleRate, AudioPlayer.Channels);

        public static FFmpegSL? ToStdout(string input, WaveFormat waveFormat)
            => ToStdout(input, waveFormat.SampleRate, waveFormat.Channels);

        public static FFmpegSL? ToStdout(string input, int sampleRate, int channels)
            => FFmpegSL.StartRaw(FFmpegArguments.ToStdout(input, sampleRate, channels), true);

        public bool TryTerminateGracefully(int timeoutMilliseconds = 1000)
        {
            if (ffmpeg.Stdin == null)
                return ffmpeg.HasExited;
            if (ffmpeg.HasExited)
                return true;
            ffmpeg.Stdin.Write('q');
            ffmpeg.Stdin.Flush();
            return ffmpeg.WaitForExit(timeoutMilliseconds);
        }

        /// <summary>
        /// Throws an exception if the FFmpeg process has exited with a non-zero exit code, and has an error message.
        /// </summary>
        /// <exception cref="FFmpegRuntimeException">The exception if validation failed.</exception>
        public void ThrowIfExitedWithError()
        {
            if (ffmpeg is {HasExited: true, ExitCode: not 0} && !string.IsNullOrWhiteSpace(ffmpeg.FinalErrorMessage))
                throw new FFmpegRuntimeException(ffmpeg.FinalErrorMessage!);
        }

    }

}
