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

        /// <summary>
        /// Caches the file if it wasn't cached after the last write to the original.
        /// </summary>
        /// <param name="path">The path to the file to cache.</param>
        /// <param name="optimizeFor">What to optimize for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// An <see cref="Awaitable"/> representing the asynchronous operation.
        /// If a cached file exists and has been written to prior to the modification of the original file, the awaitable will have completed.
        /// </returns>
        /// <remarks><see cref="File.GetLastWriteTimeUtc"/> is used for comparison.</remarks>
        public async Awaitable<SaveCacheResult> CacheIfUpdatedAsync(string path, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
            => cache.TryGetPath(path, out var cachedPath) && File.GetLastWriteTimeUtc(cachedPath) >= File.GetLastWriteTimeUtc(path)
                ? (cachedPath, null)
                : await cache.CacheAsync(path, optimizeFor, cancellationToken);

        /// <summary>
        /// Caches all files in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to search in.</param>
        /// <param name="optimizeFor">What to optimize for.</param>
        /// <param name="searchOption">Whether to search only in the directory itself, or enter subdirectories as well.</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files.
        /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation, containing the result for each file.</returns>
        /// <remarks>
        /// Canceling the token will not throw an <see cref="OperationCanceledException"/>.
        /// Instead, it will return a <see cref="CanceledError"/> for each operation that hasn't yet completed.
        /// </remarks>
        public Awaitable<SaveCacheResult[]> CacheAllAsync(
            string directory,
            OptimizeFor optimizeFor,
            SearchOption searchOption = SearchOption.TopDirectoryOnly,
            string searchPattern = "*",
            CancellationToken cancellationToken = default
        ) => cache.CacheAllAsync(Directory.EnumerateFiles(directory, searchPattern, searchOption), optimizeFor, cancellationToken);

        /// <summary>
        /// Caches all files in the specified directory.
        /// Each file will only be cached if it wasn't cached after the last write to the original.
        /// </summary>
        /// <param name="directory">The directory to search in.</param>
        /// <param name="optimizeFor">What to optimize for.</param>
        /// <param name="searchOption">Whether to search only in the directory itself, or enter subdirectories as well.</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files.
        /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation, containing the result for each file.</returns>
        /// <remarks>
        /// <see cref="File.GetLastWriteTimeUtc"/> is used for comparison.<br/>
        /// Canceling the token will not throw an <see cref="OperationCanceledException"/>.
        /// Instead, it will return a <see cref="CanceledError"/> for each operation that hasn't yet completed.
        /// </remarks>
        public async Awaitable<SaveCacheResult[]> CacheAllIfUpdatedAsync(
            string directory,
            OptimizeFor optimizeFor,
            SearchOption searchOption = SearchOption.TopDirectoryOnly,
            string searchPattern = "*",
            CancellationToken cancellationToken = default
        ) => await Task.WhenAll(
            Directory.EnumerateFiles(directory, searchPattern, searchOption)
                .Select(e => cache.CacheIfUpdatedAsync(e, optimizeFor, cancellationToken).AsTask())
        );

    }

}
