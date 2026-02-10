using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;

namespace SecretLabNAudio.FFmpeg;

public static class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioPlayer UseFFmpegSync(string input)
        {
            return player.Use(FFmpegAudioProcessor.Create(input));
        }

    }

}
