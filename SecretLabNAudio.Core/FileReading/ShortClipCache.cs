using System.Linq;
using SecretLabNAudio.Core.Extensions;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.FileReading;

/// <summary>
/// A centralized cache for storing samples by name.
/// <b>Do not use this for storing lengthy audio, stream the files instead.</b> 
/// </summary>
/// <remarks>
/// The cache is case-insensitive (ignores case).
/// Audio is automatically converted to <see cref="WaveStreamExtensions.ReadPlayerCompatibleSamples">player-compatible samples</see>.
/// </remarks>
/// <seealso cref="IAudioReaderFactory"/>
/// <seealso cref="AudioReaderFactoryManager"/>
/// <seealso cref="TryCreateAudioReader"/>
/// <seealso cref="AudioPlayerExtensions.UseShortClip"/>
/// <seealso cref="AudioPlayerExtensions.UseFile(AudioPlayer,string,bool,float)"/>
public static class ShortClipCache
{

    private static readonly Dictionary<string, RawSourceSampleProvider> Clips = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>All stored clips' keys.</summary>
    /// <remarks>Make sure not to add or remove items while iterating through this collection.</remarks>
    public static IReadOnlyCollection<string> Keys => Clips.Keys;

    /// <summary>Adds a raw sample provider to the cache.</summary>
    /// <param name="name">The key to add by.</param>
    /// <param name="provider">The clip to store.</param>
    /// <param name="trimExtension">Whether to trim the file extension from the name.</param>
    /// <exception cref="ArgumentException"><inheritdoc cref="AudioPlayer.ThrowIfIncompatible" path="exception"/></exception>
    /// <remarks>If an entry already exists, it will be overwritten.</remarks>
    public static void Add(string name, RawSourceSampleProvider provider, bool trimExtension = true)
    {
        AudioPlayer.ThrowIfIncompatible(provider);
        Clips[name.RemoveExtension(trimExtension)] = provider;
    }

    /// <summary>Removes a sample from the cache by name.</summary>
    /// <param name="name">The key to remove by.</param>
    /// <param name="provider">The resulting provider if there was a provider removed, null otherwise.</param>
    /// <param name="trimExtension">Whether to trim the file extension from the name.</param>
    /// <returns>Whether a provider was removed.</returns>
    public static bool Remove(string name, [NotNullWhen(true)] out RawSourceSampleProvider? provider, bool trimExtension = true)
        => Clips.Remove(name.RemoveExtension(trimExtension), out provider);

    /// <summary>
    /// Attempts to add the samples from the given file to the cache.
    /// <b>Do not use this for storing lengthy audio, stream the files instead.</b> 
    /// </summary>
    /// <param name="path">The path to the file.</param>
    /// <param name="trimExtension">Whether to trim the file extension from the name.</param>
    /// <returns>
    /// A <see cref="RawSourceSampleProvider"/> if the clip was added to the cache.
    /// If the file doesn't exist, no <see cref="IAudioReaderFactory">factory</see> was registered for the type, or no <see cref="WaveStream"/> was returned.
    /// </returns>
    /// <remarks>
    /// If an entry already exists, it will be overwritten.
    /// Audio is automatically converted to <see cref="WaveStreamExtensions.ReadPlayerCompatibleSamples">player-compatible samples</see>.
    /// </remarks>
    /// <seealso cref="AudioPlayerExtensions.UseFile(AudioPlayer,string,bool,float)"/>
    public static RawSourceSampleProvider? AddFromFile(string path, bool trimExtension = true)
        => AddFromFile(path, null, trimExtension);

    /// <summary><inheritdoc cref="AddFromFile(string,bool)" path="summary"/></summary>
    /// <param name="path">The path to the file.</param>
    /// <param name="maxDuration">If not null and the file's duration is longer than this value, it will not be added to the cache.</param>
    /// <param name="trimExtension">Whether to trim the file extension from the name.</param>
    /// <remarks>
    /// <inheritdoc cref="AddFromFile(string,bool)" path="remarks"/>
    /// Audio file duration may be an estimate for compressed files.
    /// </remarks>
    /// <seealso cref="AudioPlayerExtensions.UseFile(AudioPlayer,string,bool,float)"/>
    public static RawSourceSampleProvider? AddFromFile(string path, TimeSpan? maxDuration, bool trimExtension = true)
        => AddFromFile(path, path.FileName(trimExtension), maxDuration);

    /// <summary><inheritdoc cref="AddFromFile(string,bool)" path="summary"/></summary>
    /// <param name="path">The path to the file.</param>
    /// <param name="name">The key to add by.</param>
    /// <remarks><inheritdoc cref="AddFromFile(string,bool)" path="remarks"/></remarks>
    /// <seealso cref="AudioPlayerExtensions.UseFile(AudioPlayer,string,bool,float)"/>
    public static RawSourceSampleProvider? AddFromFile(string path, string name) => AddFromFile(path, name, null);

    /// <summary><inheritdoc cref="AddFromFile(string,bool)" path="summary"/></summary>
    /// <param name="path">The path to the file.</param>
    /// <param name="name">The key to add by.</param>
    /// <param name="maxDuration">If not null and the file's duration is longer than this value, the file will not be added to the cache.</param>
    /// <remarks><inheritdoc cref="AddFromFile(string,TimeSpan?,bool)"/></remarks>
    /// <seealso cref="AudioPlayerExtensions.UseFile(AudioPlayer,string,bool,float)"/>
    public static RawSourceSampleProvider? AddFromFile(string path, string name, TimeSpan? maxDuration)
    {
        if (!TryRead(path, out var provider, maxDuration))
            return null;
        Add(name, provider, false);
        return provider;
    }

    /// <summary>
    /// Attempts to add the clips from the given files to the cache with keys based on files' names.
    /// <b>Do not use this for storing lengthy audio, stream the files instead.</b> 
    /// </summary>
    /// <param name="trimExtension">Whether to trim the file extension from the names.</param>
    /// <param name="paths">The fully qualified paths to the files.</param>
    /// <returns>The number of clips added to the cache.</returns>
    /// <remarks><inheritdoc cref="AddFromFile(string,bool)"/></remarks>
    /// <seealso cref="AddFromFile(string,bool)"/>
    /// <seealso cref="AudioPlayerExtensions.UseFile(AudioPlayer,string,bool,float)"/>
    public static int AddAllFromFiles(bool trimExtension, params IEnumerable<string> paths)
        => AddAllFromFiles(trimExtension, null, paths);

    /// <summary><inheritdoc cref="AddAllFromFiles(bool,IEnumerable{string})" path="summary"/></summary>
    /// <param name="trimExtension">Whether to trim the file extension from the names.</param>
    /// <param name="maxDuration">If not null and a file's duration is longer than this value, the file will not be added to the cache.</param>
    /// <param name="paths">The fully qualified paths to the files.</param>
    /// <returns>The number of clips added to the cache.</returns>
    /// <remarks><inheritdoc cref="AddFromFile(string,TimeSpan?,bool)"/></remarks>
    /// <seealso cref="AddFromFile(string,bool)"/>
    /// <seealso cref="AudioPlayerExtensions.UseFile(AudioPlayer,string,bool,float)"/>
    public static int AddAllFromFiles(bool trimExtension, TimeSpan? maxDuration, params IEnumerable<string> paths)
    {
        var count = 0;
        foreach (var path in paths)
            if (AddFromFile(path, maxDuration, trimExtension) != null)
                count++;
        return count;
    }

    /// <summary><inheritdoc cref="AddAllFromFiles(bool,IEnumerable{string})" path="summary"/></summary>
    /// <param name="baseDirectory">The base directory to combine with the paths.</param>
    /// <param name="trimExtension">Whether to trim the file extension from the names.</param>
    /// <param name="paths">The paths to the files relative to <paramref name="baseDirectory"/>.</param>
    /// <returns>The number of clips added to the cache.</returns>
    /// <remarks><inheritdoc cref="AddFromFile(string,bool)"/></remarks>
    /// <seealso cref="AddFromFile(string,bool)"/>
    /// <seealso cref="AudioPlayerExtensions.UseFile(AudioPlayer,string,bool,float)"/>
    public static int AddAllFromFiles(string baseDirectory, bool trimExtension, params IEnumerable<string> paths)
        => AddAllFromFiles(baseDirectory, trimExtension, null, paths);

    /// <summary><inheritdoc cref="AddAllFromFiles(bool,IEnumerable{string})" path="summary"/></summary>
    /// <param name="baseDirectory">The base directory to combine with the paths.</param>
    /// <param name="trimExtension">Whether to trim the file extension from the names.</param>
    /// <param name="maxDuration">If not null and a file's duration is longer than this value, the file will not be added to the cache.</param>
    /// <param name="paths">The paths to the files relative to <paramref name="baseDirectory"/>.</param>
    /// <returns>The number of clips added to the cache.</returns>
    /// <remarks><inheritdoc cref="AddFromFile(string,TimeSpan?,bool)"/></remarks>
    /// <seealso cref="AddFromFile(string,TimeSpan?,bool)"/>
    /// <seealso cref="AudioPlayerExtensions.UseFile(AudioPlayer,string,bool,float)"/>
    public static int AddAllFromFiles(string baseDirectory, bool trimExtension, TimeSpan? maxDuration, params IEnumerable<string> paths)
        => AddAllFromFiles(trimExtension, maxDuration, paths.Select(e => Path.Combine(baseDirectory, e)));

    /// <summary>
    /// Adds all audio files from a directory to the cache with keys based on files' names.
    /// <b>Do not use this for storing lengthy audio, stream the files instead.</b>
    /// </summary>
    /// <param name="directoryPath">The directory to find files in.</param>
    /// <param name="trimExtension">Whether to trim the file extension from the names.</param>
    /// <param name="maxDuration">If not null and a file's duration is longer than this value, the file will not be added to the cache.</param>
    /// <param name="searchOption">Whether to search only in the directory itself, or enter subdirectories as well.</param>
    /// <returns>The number of clips added to the cache.</returns>
    /// <remarks><inheritdoc cref="AddFromFile(string,TimeSpan?,bool)"/></remarks>
    public static int AddAllFromDirectory(
        string directoryPath,
        bool trimExtension = true,
        TimeSpan? maxDuration = null,
        SearchOption searchOption = SearchOption.TopDirectoryOnly
    ) => AddAllFromFiles(trimExtension, maxDuration, Directory.EnumerateFiles(directoryPath, "*", searchOption));

    /// <summary>Attempts to retrieve a clip from the cache.</summary>
    /// <param name="name">The key to search by.</param>
    /// <param name="provider">The resulting provider if there was a provider found, null otherwise.</param>
    /// <param name="trimExtension">Whether to trim the file extension from the name.</param>
    /// <returns>Whether a provider was found.</returns>
    /// <remarks>This method returns a copy of the original and sets the <see cref="RawSourceSampleProvider.ClipName"/> to the key.</remarks>
    /// <seealso cref="RawSourceSampleProvider.Copy"/>
    public static bool TryGet(string name, [NotNullWhen(true)] out RawSourceSampleProvider? provider, bool trimExtension = true)
    {
        var key = name.RemoveExtension(trimExtension);
        if (!Clips.TryGetValue(key, out var original))
        {
            provider = null;
            return false;
        }

        provider = original.Copy(true);
        provider.ClipName = key;
        return true;
    }

    private static bool TryRead(string path, [NotNullWhen(true)] out RawSourceSampleProvider? provider, TimeSpan? maxDuration)
    {
        if (!File.Exists(path)
            || !TryCreateAudioReader.Stream(path, out var stream)
            || maxDuration.HasValue && stream.TotalTime > maxDuration.Value)
        {
            provider = null;
            return false;
        }

        try
        {
            provider = stream.ReadPlayerCompatibleSamples();
            return true;
        }
        finally
        {
            stream.Dispose();
        }
    }

    /// <param name="path">The path string.</param>
    extension(string path)
    {

        private string FileName(bool trimExtension)
            => trimExtension
                ? Path.GetFileNameWithoutExtension(path)
                : Path.GetFileName(path);

        private string RemoveExtension(bool trimExtension)
        {
            if (trimExtension)
                path = Path.ChangeExtension(path, null);
            return path;
        }

    }

}
