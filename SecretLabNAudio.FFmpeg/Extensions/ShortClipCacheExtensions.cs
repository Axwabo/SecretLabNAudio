using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static partial class ShortClipCacheExtensions
{

    extension(ShortClipCache)
    {

        public static RawSourceSampleProvider? AddWithFFmpeg(string input, TimeSpan? maxDuration = null)
        {
            if (ReadWithFFmpeg(input, maxDuration) is not { } provider)
                return null;
            ShortClipCache.Add((input, false), provider);
            return provider;
        }

        public static RawSourceSampleProvider? AddWithFFmpeg(FFmpegArguments arguments, ClipName clipName, TimeSpan? maxDuration = null)
        {
            if (ReadWithFFmpeg(arguments, maxDuration) is not { } provider)
                return null;
            ShortClipCache.Add(clipName, provider);
            return provider;
        }

    }

}
