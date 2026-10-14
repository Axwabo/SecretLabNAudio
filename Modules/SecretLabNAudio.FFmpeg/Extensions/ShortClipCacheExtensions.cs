using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Providers;

// ReSharper disable InvokeAsExtensionMember

namespace SecretLabNAudio.FFmpeg.Extensions;

/// <summary>
/// FFmpeg-based extensions for the <see cref="ShortClipCache"/>.
/// </summary>
public static partial class ShortClipCacheExtensions
{

    extension(ShortClipCache)
    {

        /// <include file="../XmlDocs/Clips.xml" path="doc/summary"/>
        /// <param name="path">The path to the file.</param>
        /// <param name="maxDuration">If not null and the file's duration is longer than this value, it will not be added to the cache.</param>
        /// <param name="trimExtension">Whether to trim the file extension from the name.</param>
        /// <include file="../XmlDocs/Clips.xml" path="doc/returns"/>
        /// <include file="../XmlDocs/Clips.xml" path="doc/remarks"/>
        /// <include file="../XmlDocs/Clips.xml" path="doc/seealso"/>
        public static RawSourceSampleProvider? AddFileWithFFmpeg(string path, TimeSpan? maxDuration = null, bool trimExtension = true)
            => AddFileWithFFmpeg(path, ClipName.FromPath(path, trimExtension), maxDuration);

        /// <include file="../XmlDocs/Clips.xml" path="doc/summary"/>
        /// <param name="path">The path to the file.</param>
        /// <param name="clipName">The key to add by.</param>
        /// <param name="maxDuration">If not null and the file's duration is longer than this value, it will not be added to the cache.</param>
        /// <include file="../XmlDocs/Clips.xml" path="doc/returns"/>
        /// <include file="../XmlDocs/Clips.xml" path="doc/remarks"/>
        /// <include file="../XmlDocs/Clips.xml" path="doc/seealso"/>
        public static RawSourceSampleProvider? AddFileWithFFmpeg(string path, ClipName clipName, TimeSpan? maxDuration = null)
        {
            if (!File.Exists(path) || ReadWithFFmpeg(path, maxDuration) is not { } provider)
                return null;
            ShortClipCache.Add(clipName, provider);
            return provider;
        }

        /// <include file="../XmlDocs/Clips.xml" path="doc/summary"/>
        /// <param name="arguments">The arguments to pass to FFmpeg.</param>
        /// <param name="clipName">The key to add by.</param>
        /// <param name="maxDuration">If not null and the file's duration is longer than this value, it will not be added to the cache.</param>
        /// <include file="../XmlDocs/Clips.xml" path="doc/returns"/>
        /// <include file="../XmlDocs/Clips.xml" path="doc/remarks"/>
        /// <include file="../XmlDocs/Clips.xml" path="doc/seealso"/>
        /// <include file="../XmlDocs/Args.xml" path="doc/In/exception"/>
        public static RawSourceSampleProvider? AddWithFFmpeg(FFmpegArguments arguments, ClipName clipName, TimeSpan? maxDuration = null)
        {
            if (ReadWithFFmpeg(arguments, maxDuration) is not { } provider)
                return null;
            ShortClipCache.Add(clipName, provider);
            return provider;
        }

        /// <summary>
        /// Adds all audio files from a directory to the cache using FFmpeg with keys based on files' names.
        /// <b>Do not use this for storing lengthy audio, stream the files instead.</b>
        /// </summary>
        /// <param name="directoryPath">The directory to find files in.</param>
        /// <param name="trimExtension">Whether to trim the file extension from the names.</param>
        /// <param name="maxDuration">If not null and a file's duration is longer than this value, the file will not be added to the cache.</param>
        /// <param name="searchOption">Whether to search only in the directory itself, or enter subdirectories as well.</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files.
        /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.
        /// </param>
        /// <returns>The number of clips added to the cache.</returns>
        /// <include file="../XmlDocs/Clips.xml" path="doc/remarks"/>
        /// <seealso cref="Directory.EnumerateFiles(string,string,SearchOption)"/>
        /// <include file="../XmlDocs/Clips.xml" path="doc/seealso"/>
        public static int AddAllFromDirectoryWithFFmpeg(
            string directoryPath,
            bool trimExtension = true,
            TimeSpan? maxDuration = null,
            SearchOption searchOption = SearchOption.TopDirectoryOnly,
            string searchPattern = "*"
        )
        {
            var count = 0;
            foreach (var path in Directory.EnumerateFiles(directoryPath, searchPattern, searchOption))
                if (AddFileWithFFmpeg(path, ClipName.FromPath(path, trimExtension), maxDuration) != null)
                    count++;
            return count;
        }

    }

}
