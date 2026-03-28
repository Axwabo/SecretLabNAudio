using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Caches;

/// <summary>
/// A base class for caching optimized audio files.
/// </summary>
/// <typeparam name="TSource">The type of input to accept from callers.</typeparam>
/// <typeparam name="TKey">The type of key to generate the file path with.</typeparam>
public abstract class AudioCacheBase<TSource, TKey>
{

    /// <summary>
    /// Initializes a new cache, and creates the directory if necessary.
    /// </summary>
    /// <param name="folder">The directory to save files to.</param>
    protected AudioCacheBase(string folder)
    {
        Folder = folder;
        Directory.CreateDirectory(folder);
    }

    /// <summary>
    /// Initializes a new cache, and creates the directory if necessary.
    /// </summary>
    /// <param name="directoryInfo">The directory to save files to.</param>
    protected AudioCacheBase(DirectoryInfo directoryInfo) : this(directoryInfo.FullName)
    {
    }

    /// <summary>
    /// The directory to save files to.
    /// </summary>
    public string Folder { get; }

    /// <summary>
    /// Gets the key corresponding to the source.
    /// </summary>
    /// <param name="source">The source to convert.</param>
    /// <returns>The key associated with the source.</returns>
    public abstract TKey GetKey(TSource source);

    /// <summary>
    /// Gets fully qualified output path given a key and an optimization target.
    /// </summary>
    /// <param name="key">The key to save by.</param>
    /// <param name="optimizeFor">What to optimize for.</param>
    /// <returns>A fully qualified path to the cached file.</returns>
    public string GetOutput(TKey key, OptimizeFor optimizeFor) => Path.Combine(Folder, $"{key}.{optimizeFor.Extension}");

    /// <summary>
    /// Asynchronously starts and waits for FFmpeg to perform caching.
    /// </summary>
    /// <param name="source">The object to save by.</param>
    /// <param name="optimizeFor">What to optimize for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation.</returns>
    public abstract Awaitable<SaveCacheResult> CacheAsync(TSource source, OptimizeFor optimizeFor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to get the cached path of a source.
    /// </summary>
    /// <param name="source">The object to find the value by.</param>
    /// <param name="cachedPath">The fully qualified path if a cached file was found, null otherwise.</param>
    /// <returns>Whether a cached file was found.</returns>
    public virtual bool TryGetPath(TSource source, [NotNullWhen(true)] out string? cachedPath)
    {
        var key = GetKey(source);
        var speed = GetOutput(key, OptimizeFor.ReadingSpeed);
        if (File.Exists(speed))
        {
            cachedPath = speed;
            return true;
        }

        var size = GetOutput(key, OptimizeFor.FileSize);
        if (File.Exists(size))
        {
            cachedPath = size;
            return true;
        }

        cachedPath = null;
        return false;
    }

}
