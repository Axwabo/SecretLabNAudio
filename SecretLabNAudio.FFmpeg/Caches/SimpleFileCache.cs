using LabApi.Loader.Features.Paths;
using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Caches;

/// <summary>
/// A simple cache to help generate optimized media from audio files.
/// </summary>
public sealed class SimpleFileCache : AudioCacheBase<string, int>
{

    /// <summary>
    /// The template arguments (sample rate, channels, output options) used for caching.
    /// </summary>
    public static readonly FFmpegArguments ArgumentsTemplate = new()
    {
        SampleRate = AudioPlayer.SampleRate,
        Channels = AudioPlayer.Channels,
        OutputOptions = "-y -vn"
    };

    /// <summary>
    /// A shared <see cref="SimpleFileCache"/> instance.
    /// </summary>
    public static SimpleFileCache Shared { get; } = new(PathManager.Plugins.CreateSubdirectory("global").CreateSubdirectory("SecretLabNAudio.FFmpeg").CreateSubdirectory("Cache"));

    /// <inheritdoc/>
    public SimpleFileCache(string folder) : base(folder)
    {
    }

    /// <inheritdoc/>
    public SimpleFileCache(DirectoryInfo directoryInfo) : base(directoryInfo)
    {
    }

    /// <summary>
    /// Gets the key from the fully qualified path.
    /// </summary>
    /// <param name="fullSource">The absolute path to the file.</param>
    /// <returns>The key associated with the path.</returns>
    public override int GetKey(string fullSource) => fullSource.GetStableHashCode();

    /// <summary>
    /// Asynchronously starts and waits for FFmpeg to cache the file.
    /// </summary>
    /// <param name="source">The path to the file to cache.</param>
    /// <param name="optimizeFor">What to optimize for.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="Awaitable"/> representing the asynchronous operation.</returns>
    public override async Awaitable<SaveCacheResult> CacheAsync(string source, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(source) || source.Contains('"'))
            return ("", new InvalidInputError(source));
        var fullSource = Path.GetFullPath(source);
        var key = GetKey(fullSource);
        var output = GetOutput(key, optimizeFor);
        if (!File.Exists(fullSource))
            return (output, new FileNotFoundError(fullSource));
        await Awaitable.BackgroundThreadAsync();
        using var ffmpeg = FFmpegSL.Start(ArgumentsTemplate with {Input = fullSource, Output = output});
        if (ffmpeg == null)
            return (output, FFmpegSL.LastCaughtStartError);
        if (!await ffmpeg.WaitForExitAsync(cancellationToken).ConfigureAwait(false))
            return (output, SaveCacheError.Canceled);
        if (ffmpeg.HasExitedWithError)
            return (output, new FFmpegRuntimeError(ffmpeg.FinalErrorMessage!));
        await WriteMetadataAsync(fullSource, output, cancellationToken);
        return (output, null);
    }

    /// <summary>
    /// Attempts to get the cached path of a file.
    /// </summary>
    /// <param name="source">The file path to find the cached path by.</param>
    /// <param name="cachedPath">The fully qualified path if a cached file was found, null otherwise.</param>
    /// <returns>Whether a cached path was found.</returns>
    /// <remarks><see cref="OptimizeFor.ReadingSpeed"/> is checked first, then <see cref="OptimizeFor.FileSize"/>.</remarks>
    public override bool TryGetPath(string source, [NotNullWhen(true)] out string? cachedPath)
    {
        if (File.Exists(source))
            return base.TryGetPath(Path.GetFullPath(source), out cachedPath);
        cachedPath = null;
        return false;
    }

    private static async Awaitable WriteMetadataAsync(string fullSource, string output, CancellationToken cancellationToken)
    {
        try
        {
            await File.WriteAllTextAsync($"{output}.path", fullSource, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write metadata for the file cached from {fullSource}");
            Debug.LogException(e);
        }
    }

}
