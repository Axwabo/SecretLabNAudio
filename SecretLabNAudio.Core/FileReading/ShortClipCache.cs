using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using SecretLabNAudio.Core.Providers;

namespace SecretLabNAudio.Core.FileReading;

/// <summary>
/// A centralized cache for storing samples by name.
/// <b>Do not use this for storing lengthy audio, stream the files instead.</b> 
/// </summary>
/// <remarks>
/// The cache is case-isensitive (ignores case).
/// Audio is automatically converted to <see cref="WaveStreamExtensions.ReadPlayerCompatibleSamples">player-compatible samples</see>.
/// </remarks>
/// <seealso cref="IAudioReaderFactory"/>
/// <seealso cref="AudioReaderFactoryManager"/>
/// <seealso cref="TryCreateAudioReader"/>
/// <seealso cref="AudioPlayerExtensions.AddMixerShortClip"/>
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
    public static RawSourceSampleProvider? AddFromFile(string path, bool trimExtension = true)
    {
        if (!TryRead(path, out var provider))
            return null;
        Add(path.FileName(trimExtension), provider, trimExtension);
        return provider;
    }

    /// <inheritdoc cref="AddFromFile(string,bool)"/>
    /// <param name="path">The path to the file.</param>
    /// <param name="name">The key to add by.</param>
    public static RawSourceSampleProvider? AddFromFile(string path, string name)
    {
        if (!TryRead(path, out var provider))
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
    public static int AddAllFromFiles(bool trimExtension, params IEnumerable<string> paths)
    {
        var count = 0;
        foreach (var path in paths)
            if (AddFromFile(path, trimExtension) != null)
                count++;
        return count;
    }

    /// <inheritdoc cref="AddAllFromFiles(bool,IEnumerable{string})"/>
    /// <param name="baseDirectory">The base directory to combine with the paths.</param>
    /// <param name="trimExtension">Whether to trim the file extension from the names.</param>
    /// <param name="paths">The paths to the files relative to <paramref name="baseDirectory"/>.</param>
    public static int AddAllFromFiles(string baseDirectory, bool trimExtension, params IEnumerable<string> paths)
        => AddAllFromFiles(trimExtension, paths.Select(e => Path.Combine(baseDirectory, e)));

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

    private static bool TryRead(string path, [NotNullWhen(true)] out RawSourceSampleProvider? provider)
    {
        if (!File.Exists(path) || !TryCreateAudioReader.Stream(path, out var stream))
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

    private static string FileName(this string path, bool trimExtension)
        => trimExtension
            ? Path.GetFileNameWithoutExtension(path)
            : Path.GetFileName(path);

    private static string RemoveExtension(this string name, bool trimExtension)
    {
        if (trimExtension)
            name = Path.ChangeExtension(name, null);
        return name;
    }

}
