using SecretLabNAudio.Core;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.FFmpeg.Processors;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static class AudioPlayerExtensions
{

    extension(AudioPlayer player)
    {

        public AudioPlayer UseFFmpegSync(string input)
        {
            return player.Use(SynchronousFFmpegAudioProcessor.CreatePlayerCompatible(input));
        }

    }

}
