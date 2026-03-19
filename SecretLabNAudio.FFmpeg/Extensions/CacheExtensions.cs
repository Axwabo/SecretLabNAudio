using System.Linq;
using System.Threading.Tasks;
using SecretLabNAudio.FFmpeg.Caches;

namespace SecretLabNAudio.FFmpeg.Extensions;

/// <summary>
/// Extension members for FFmpeg-based caches.
/// </summary>
public static class CacheExtensions
{

    extension<T>(Awaitable<T> awaitable)
    {

        private async Task<T> AsTask() => await awaitable;

    }

    extension(OptimizeFor optimizeFor)
    {

        /// <summary>
        /// The file extension to save as.
        /// </summary>
        public string Extension => optimizeFor == OptimizeFor.FileSize ? "ogg" : "wav";

    }

    /// <param name="cache">The cache to perform operations on.</param>
    /// <typeparam name="TSource">The type of input to accept.</typeparam>
    /// <typeparam name="TKey">The type of key to generate the file path with.</typeparam>
    extension<TSource, TKey>(AudioCacheBase<TSource, TKey> cache)
    {

        /// <summary>
        /// Caches all given sources in parallel.
        /// </summary>
        /// <param name="sources">The sources to cache.</param>
        /// <param name="optimizeFor">What to optimize for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation, containing the results in the order of <paramref name="sources"/>.</returns>
        public async Awaitable<SaveCacheResult[]> CacheAllAsync(IEnumerable<TSource> sources, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => await Task.WhenAll(sources.Select(e => cache.CacheAsync(e, optimizeFor, cancellationToken).AsTask()));

    }

    /// <param name="cache">The cache to perform operations on.</param>
    extension(SimpleFileCache cache)
    {

        /// <summary>
        /// Gets the fully qualified path to a cached file, or the original path.
        /// </summary>
        /// <param name="path">The path to convert.</param>
        /// <returns>The fully qualified path to a cached file if it was cached, the original path otherwise.</returns>
        public string GetPathOrFallback(string path)
            => cache.TryGetPath(path, out var cachedPath) ? cachedPath : path;

        public async Awaitable<SaveCacheResult> CacheIfUpdatedAsync(string path, OptimizeFor optimizeFor)
            => cache.TryGetPath(path, out var cachedPath) && File.GetLastWriteTimeUtc(cachedPath) >= File.GetLastWriteTimeUtc(path)
                ? (cachedPath, null)
                : await cache.CacheAsync(path, optimizeFor);

        public Awaitable<SaveCacheResult[]> CacheAllAsync(string directory, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.CacheAllAsync(Directory.EnumerateFiles(directory), optimizeFor, cancellationToken);

        public async Awaitable<SaveCacheResult[]> CacheAllIfUpdatedAsync(string directory, OptimizeFor optimizeFor)
            => await Task.WhenAll(Directory.EnumerateFiles(directory).Select(e => cache.CacheIfUpdatedAsync(e, optimizeFor).AsTask()));

    }

}
