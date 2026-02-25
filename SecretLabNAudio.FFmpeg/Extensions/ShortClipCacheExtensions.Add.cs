using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static partial class ShortClipCacheExtensions
{

    extension(ShortClipCache)
    {

        public static RawSourceSampleProvider? AddWithFFmpeg(string input, TimeSpan? maxDuration = null) => ReadWithFFmpeg(input, maxDuration);

        public static RawSourceSampleProvider? AddWithFFmpeg(FFmpegArguments arguments, TimeSpan? maxDuration = null) => ReadWithFFmpeg(arguments, maxDuration);

    }

}
