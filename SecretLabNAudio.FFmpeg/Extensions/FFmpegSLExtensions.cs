using SecretLabNAudio.Core;
using SecretLabNAudio.FFmpeg.Interop;

// ReSharper disable InvokeAsExtensionMember

namespace SecretLabNAudio.FFmpeg.Extensions;

public static class FFmpegSLExtensions
{
    extension(FFmpegSL)
    {

        public static FFmpegSL? PlayerCompatibleToStdout(string input)
            => ToStdout(input, AudioPlayer.SampleRate, AudioPlayer.Channels);

        public static FFmpegSL? ToStdout(string input, WaveFormat waveFormat)
            => ToStdout(input, waveFormat.SampleRate, waveFormat.Channels);

        public static FFmpegSL? ToStdout(string input, int sampleRate, int channels)
            => FFmpegSL.StartRaw($"-v error -i \"{input}\" -ar {sampleRate} -ac {channels} -f f32le -", true);

    }

}
