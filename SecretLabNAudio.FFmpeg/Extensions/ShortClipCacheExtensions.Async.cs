using System.Linq;
using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.FFmpeg.Extensions;

public static partial class ShortClipCacheExtensions
{

    public static async Awaitable<RawSourceSampleProvider?> ReadWithFFmpegAsync(string path, TimeSpan? maxDuration)
    {
        try
        {
            await Awaitable.BackgroundThreadAsync();
            return ReadWithFFmpeg(path, maxDuration);
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return null; // TODO: throw or null?
        }
    }

    public static async Awaitable<RawSourceSampleProvider?[]> ReadWithFFmpegAsync(IEnumerable<string> paths, TimeSpan? maxDuration)
    {
        await Awaitable.MainThreadAsync();
        return await Awaitable.WhenAll(paths.Select(e => ReadWithFFmpegAsync(e, maxDuration)));
    }

    extension(ShortClipCache)
    {

        public static async Awaitable<RawSourceSampleProvider?[]> AddAllWithFFmpegAsync(IEnumerable<string> paths, TimeSpan? maxDuration)
        {
            var array = await ReadWithFFmpegAsync(paths, maxDuration);
            foreach (var provider in array)
            {
                // TODO: paths
            }

            return array;
        }

    }

}
