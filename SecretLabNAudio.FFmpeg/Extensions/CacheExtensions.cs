using System.Linq;
using System.Threading.Tasks;
using SecretLabNAudio.FFmpeg.Caches;

namespace SecretLabNAudio.FFmpeg.Extensions;

using Results = Awaitable<(string Output, SaveCacheError?)[]>;

public static class CacheExtensions
{

    extension<T>(Awaitable<T> awaitable)
    {

        private async Task<T> AsTask() => await awaitable;

    }

    extension(OptimizeFor optimizeFor)
    {

        public string Extension => optimizeFor == OptimizeFor.FileSize ? "ogg" : "wav";

    }

    extension(SimpleFileCache cache)
    {

        public string GetPathOrFallback(string path)
            => cache.TryGetPath(path, out var cachedPath) ? cachedPath : path;

        public async Awaitable<(string Output, SaveCacheError?)> CacheIfUpdatedAsync(string path, OptimizeFor optimizeFor)
            => cache.TryGetPath(path, out var cachedPath) && File.GetLastWriteTimeUtc(cachedPath) >= File.GetLastWriteTimeUtc(path)
                ? (cachedPath, null)
                : await cache.CacheAsync(path, optimizeFor);

        public async Results CacheAllAsync(string directory, OptimizeFor optimizeFor)
            => await Task.WhenAll(Directory.EnumerateFiles(directory).Select(e => cache.CacheAsync(e, optimizeFor).AsTask()));

        public async Results CacheAllIfUpdatedAsync(string directory, OptimizeFor optimizeFor)
            => await Task.WhenAll(Directory.EnumerateFiles(directory).Select(e => cache.CacheIfUpdatedAsync(e, optimizeFor).AsTask()));

    }

}
