using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Extensions.Processors;
using SecretLabNAudio.FFmpeg.Caches;
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

        /// <summary>
        /// Uses the <see cref="SimpleFileCache.Shared">simple file cache</see> to play an optimized file, or falls back to playing the original file.
        /// </summary>
        /// <param name="path">The path to the original file.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <param name="volume">The volume of the input.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>The stream will be converted to be player-compatible.</remarks>
        /// <seealso cref="Core.Extensions.AudioPlayerExtensions.UseFile(AudioPlayer,string,bool,float)"/>
        /// <include file='../XmlDocs/Files.xml' path='doc/NotFound'/>
        public AudioPlayer UseCachedFile(string path, bool loop = false, float volume = 1)
            => player.UseFile(SimpleFileCache.Shared.GetPathOrFallback(path), loop, volume);

        /// <summary>
        /// Uses the <see cref="SimpleFileCache.Shared">simple file cache</see> to play an optimized file, or falls back to playing the original file.
        /// </summary>
        /// <param name="path">The path to the original file.</param>
        /// <param name="modify">A delegate to process the provider. If null, a <see cref="Core.Processors.ProcessorChain"/> will only be created if format conversion is required.</param>
        /// <param name="loop">Whether to loop the file.</param>
        /// <returns>The player itself.</returns>
        /// <remarks>The stream will be converted to be player-compatible.</remarks>
        /// <seealso cref="Core.Extensions.AudioPlayerExtensions.UseFile(AudioPlayer,string,bool,float)"/>
        /// <include file='../XmlDocs/Files.xml' path='doc/NotFound'/>
        public AudioPlayer UseCachedFile(string path, ModifyChain? modify, bool loop = false)
            => player.UseFile(SimpleFileCache.Shared.GetPathOrFallback(path), modify, loop);

        public AudioPlayer UseCachedFileSafe(string path, bool loop = false, float volume = 1)
            => player.UseFileSafe(SimpleFileCache.Shared.GetPathOrFallback(path), loop, volume);

        public AudioPlayer UseCachedFileSafe(string path, ModifyChain? modifyChain, bool loop = false)
            => player.UseFileSafe(SimpleFileCache.Shared.GetPathOrFallback(path), modifyChain, loop);

    }

}
