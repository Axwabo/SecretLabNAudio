using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors.Playlists;
using SecretLabNAudio.FFmpeg.Caches;

namespace SecretLabNAudio.FFmpeg.Processors;

/// <summary>
/// A queued playlist item using a cached version if available.
/// </summary>
/// <param name="Source">The input to look for in the cache.</param>
/// <param name="Name">The name of the item.</param>
/// <typeparam name="TSource">The type of input the cache accepts.</typeparam>
/// <typeparam name="TKey">The type of key the cache generates the file path with.</typeparam>
public abstract record CachedPlaylistItem<TSource, TKey>(TSource Source, string? Name = null) : PlaylistItem(Name)
{

    /// <summary>
    /// Gets the cache instance to use for file lookups.
    /// </summary>
    /// <returns>A cache instance.</returns>
    protected abstract AudioCacheBase<TSource, TKey> GetCache();

    /// <summary>Creates a sample provider when there was no cache hit.</summary>
    /// <inheritdoc cref="PlaylistItem.CreateProvider"/>
    protected abstract ISampleProvider CreateFallback(int sampleRate, int channels);

    /// <inheritdoc/>
    protected sealed override ISampleProvider CreateProvider(int sampleRate, int channels)
        => GetCache().TryGetPath(Source, out var path)
            ? CreateAudioProcessor.FromFile(path)
            : CreateFallback(sampleRate, channels);

}

/// <summary>
/// A queued playlist item that uses a cached version of a file if available.
/// </summary>
/// <param name="FilePath">The path to the file.</param>
public sealed record CachedFilePlaylistItem(string FilePath) : CachedPlaylistItem<string, int>(FilePath, Path.GetFileNameWithoutExtension(FilePath))
{

    /// <inheritdoc/>
    protected override AudioCacheBase<string, int> GetCache() => SimpleFileCache.Shared;

    /// <inheritdoc/>
    protected override ISampleProvider CreateFallback(int sampleRate, int channels)
        => CreateAudioProcessor.FromFile(FilePath);

}
