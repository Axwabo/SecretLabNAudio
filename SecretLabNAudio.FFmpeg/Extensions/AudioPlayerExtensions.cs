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
        /// <param name="capacity">The capacity of the buffer in seconds.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Args.xml' path='doc/InArg/exception'/>
        /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
        public AudioPlayer UseFFmpeg(string input, double capacity = AsyncFFmpegProcessorBase.DefaultCapacity)
            => player.Use(AsyncBufferedFFmpegAudioProcessor.CreatePlayerCompatible(input, capacity));

        /// <summary>
        /// Sets the <see cref="AudioPlayer.SampleProvider"/> to a new <see cref="AsyncBufferedFFmpegAudioProcessor"/>.
        /// </summary>
        /// <param name="arguments">The arguments to pass to FFmpeg.</param>
        /// <param name="capacity">The capacity of the buffer in seconds.</param>
        /// <returns>The player itself.</returns>
        /// <include file='../XmlDocs/Args.xml' path='doc/In/exception'/>
        /// <include file='../XmlDocs/Buffered.xml' path='doc/Capacity/remarks'/>
        public AudioPlayer UseFFmpeg(FFmpegArguments arguments, double capacity = AsyncFFmpegProcessorBase.DefaultCapacity)
            => player.Use(AsyncBufferedFFmpegAudioProcessor.CreatePlayerCompatible(arguments, capacity));

    }

}
