using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Caches;

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

    public virtual bool TryGetPath(TSource source, [NotNullWhen(true)] out string? cachedPath)
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
