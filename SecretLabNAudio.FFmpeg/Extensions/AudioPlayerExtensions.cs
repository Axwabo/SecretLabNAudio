using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.FFmpeg.Processors;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioPlayer UseFFmpeg(string input) => player.Use(AsyncBufferedFFmpegAudioProcessor.CreatePlayerCompatible(input));

    }

}
