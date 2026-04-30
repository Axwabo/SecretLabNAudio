using SecretLabNAudio.Core.FileReading;
using SecretLabNAudio.Core.Processors.Playlists;
using SecretLabNAudio.FFmpeg.Caches;

namespace SecretLabNAudio.FFmpeg.Processors;

public abstract record CachedPlaylistItem<TSource, TKey>(TSource Source, string? Name = null) : PlaylistItem(Name)
{

    protected abstract AudioCacheBase<TSource, TKey> GetCache();

    protected abstract ISampleProvider CreateFallback(int sampleRate, int channels);

    public sealed override ISampleProvider CreateProvider(int sampleRate, int channels)
        => GetCache().TryGetPath(Source, out var path)
            ? CreateAudioProcessor.FromFile(path)
            : CreateFallback(sampleRate, channels);

}

public sealed record CachedFilePlaylistItem(string FilePath) : CachedPlaylistItem<string, int>(FilePath, Path.GetFileNameWithoutExtension(FilePath))
{

    protected override AudioCacheBase<string, int> GetCache() => SimpleFileCache.Shared;

    protected override ISampleProvider CreateFallback(int sampleRate, int channels)
        => CreateAudioProcessor.FromFile(FilePath);

}
