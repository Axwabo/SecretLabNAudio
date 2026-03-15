using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Caches;

/// <summary>
/// A base class for caching optimized audio files.
/// </summary>
/// <typeparam name="TSource">The type of input to accept from callers.</typeparam>
/// <typeparam name="TKey">The type of key to generate the file path with.</typeparam>
public abstract class AudioCacheBase<TSource, TKey>
{

    protected AudioCacheBase(string folder)
    {
        Folder = folder;
        Directory.CreateDirectory(folder);
    }

    protected AudioCacheBase(DirectoryInfo directoryInfo) : this(directoryInfo.FullName)
    {
    }

    public string Folder { get; }

    protected abstract TKey GetKey(TSource source);

    protected string Output(TKey key, OptimizeFor optimizeFor) => Path.Combine(Folder, $"{key}.{optimizeFor.Extension}");

    /// <summary>
    /// Attempts to get the cached path of a source.
    /// </summary>
    /// <param name="source">The object to find the value by.</param>
    /// <param name="cachedPath">The fully qualified path if a cached file was found, null otherwise.</param>
    /// <returns>Whether a cached path was found.</returns>
    /// <remarks>Inheriting classes must define a new method to expose this with validation.</remarks>
    protected bool TryGetPath(TSource source, [NotNullWhen(true)] out string? cachedPath)
    {
        var key = GetKey(source);
        var speed = Output(key, OptimizeFor.ReadingSpeed);
        if (File.Exists(speed))
        {
            cachedPath = speed;
            return true;
        }

        var size = Output(key, OptimizeFor.FileSize);
        if (File.Exists(size))
        {
            cachedPath = size;
            return true;
        }

        cachedPath = null;
        return false;
    }

}
