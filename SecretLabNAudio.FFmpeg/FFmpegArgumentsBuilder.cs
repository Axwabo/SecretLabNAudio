using SecretLabNAudio.Core;

namespace SecretLabNAudio.FFmpeg;

public static class FFmpegArgumentsBuilder
{

    public static string PlayerCompatibleToStdout(string input) => ToStdout(input, AudioPlayer.SampleRate, AudioPlayer.Channels);

    public static string ToStdout(string input, int sampleRate, int channels)
        => $"-i \"{input}\" -ar {sampleRate} -ac {channels} -f f32le -";

    public static string ToStdout(string input, WaveFormat waveFormat)
        => $"-i \"{input}\" -ar {waveFormat.SampleRate} -ac {waveFormat.Channels} -f f32le -";

}
