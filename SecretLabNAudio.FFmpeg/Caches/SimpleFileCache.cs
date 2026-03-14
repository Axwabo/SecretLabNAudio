using LabApi.Loader.Features.Paths;
using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Caches;

public sealed class SimpleFileCache : AudioCacheBase<string, int>
{

    private static readonly FFmpegArguments Template = new()
    {
        SampleRate = AudioPlayer.SampleRate,
        Channels = AudioPlayer.Channels,
        OutputOptions = "-y"
    };

    public static SimpleFileCache Shared { get; } = new(PathManager.Plugins.CreateSubdirectory("global").CreateSubdirectory("SecretLabNAudio.FFmpeg").CreateSubdirectory("Cache"));

    public SimpleFileCache(string folder) : base(folder)
    {
    }

    public SimpleFileCache(DirectoryInfo directoryInfo) : base(directoryInfo)
    {
    }

    protected override int GetKey(string fullSource) => fullSource.GetStableHashCode();

    public async Awaitable<(string OutputPath, SaveCacheError? Error)> CacheAsync(string source, OptimizeFor optimizeFor, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(source) || source.Contains('"'))
            return ("", new InvalidInputError(source));
        var fullSource = Path.GetFullPath(source);
        var key = GetKey(fullSource);
        var output = Output(key, optimizeFor);
        if (!File.Exists(fullSource))
            return (output, new FileNotFoundError(fullSource));
        await Awaitable.BackgroundThreadAsync();
        using var ffmpeg = FFmpegSL.Start(Template with {Input = fullSource, Output = output});
        if (ffmpeg == null)
            return (output, new FFmpegStartupError(FFmpegSL.LastCaughtStartError));
        if (!await ffmpeg.WaitForExitAsync(cancellationToken).ConfigureAwait(false))
            return (output, SaveCacheError.Canceled);
        if (ffmpeg.HasExitedWithError)
            return (output, new FFmpegRuntimeError(ffmpeg.FinalErrorMessage!));
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

        return (output, null);
    }

    public new bool TryGetPath(string source, [NotNullWhen(true)] out string? cachedPath)
    {
        if (File.Exists(source))
            return base.TryGetPath(Path.GetFullPath(source), out cachedPath);
        cachedPath = null;
        return false;
    }

}
