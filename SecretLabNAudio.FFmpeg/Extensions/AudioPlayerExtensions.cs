using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.FFmpeg.Processors;

namespace SecretLabNAudio.FFmpeg.Extensions;

/// <summary>
/// FFmpeg-based extension methods for the <see cref="AudioPlayer"/> class. 
/// </summary>
public static class AudioPlayerExtensions
{

    /// <param name="player">The player to modify.</param>
    extension(AudioPlayer player)
    {

        /// <summary>
        /// Sets the <see cref="AudioPlayer.SampleProvider"/> to a new <see cref="AsyncBufferedFFmpegAudioProcessor"/>.
        /// </summary>
        /// <param name="input">The input source (e.g. file path, URL).</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Args.xml' path='doc/InArg/exception'/>
        public AudioPlayer UseFFmpeg(string input)
            => player.Use(AsyncBufferedFFmpegAudioProcessor.CreatePlayerCompatible(input));

        /// <summary>
        /// Sets the <see cref="AudioPlayer.SampleProvider"/> to a new <see cref="AsyncBufferedFFmpegAudioProcessor"/>.
        /// </summary>
        /// <param name="arguments">The arguments to pass to FFmpeg.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Args.xml' path='doc/In/exception'/>
        public AudioPlayer UseFFmpeg(FFmpegArguments arguments)
            => player.Use(AsyncBufferedFFmpegAudioProcessor.CreatePlayerCompatible(arguments));

    }

}
