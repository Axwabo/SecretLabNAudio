using LabApi.Loader.Features.Paths;
using SecretLabNAudio.FFmpeg.Extensions;

namespace SecretLabNAudio.FFmpeg.Caches;

public sealed class SimpleFileCache
{

    private static readonly FFmpegArguments Template = new()
    {
        SampleRate = AudioPlayer.SampleRate,
        Channels = AudioPlayer.Channels
    };

    public static SimpleFileCache Shared { get; } = new(PathManager.Configs.CreateSubdirectory("global").CreateSubdirectory("SecretLabNAudio.FFmpeg").CreateSubdirectory("Cache"));

    public string Folder { get; }

    public SimpleFileCache(string folder)
    {
        Folder = folder;
        Directory.CreateDirectory(folder);
    }

    public SimpleFileCache(DirectoryInfo directoryInfo) : this(directoryInfo.FullName)
    {
    }

    private string Output(int key, OptimizeFor optimizeFor) => Path.Combine(Folder, $"{key}.{optimizeFor.Extension}");

    public async Awaitable<(string OutputPath, SaveCacheError? Error)> CacheAsync(string source, OptimizeFor optimizeFor)
    {
        if (string.IsNullOrWhiteSpace(source) || source.Contains('"'))
            return ("", new InvalidInputError(source));
        var fullSource = Path.GetFullPath(source);
        var key = fullSource.GetStableHashCode();
        var output = Output(key, optimizeFor);
        if (!File.Exists(fullSource))
            return (output, new FileNotFoundError(fullSource));
        // await Awaitable.BackgroundThreadAsync();
        using var ffmpeg = FFmpegSL.Start(Template with {Input = fullSource, Output = output});
        if (ffmpeg == null)
            return (output, new FFmpegStartupError(FFmpegSL.LastCaughtStartError));
        // this is so cooked
        // await Awaitable.MainThreadAsync();
        // while (!ffmpeg.HasExited)
        // await Awaitable.NextFrameAsync();
        // await Awaitable.BackgroundThreadAsync();
        ffmpeg.WaitForExit(10000);
        // ffmpeg.WaitForExit();
        if (ffmpeg.HasExitedWithError)
            return (output, new FFmpegRuntimeError(ffmpeg.FinalErrorMessage!));
        await File.WriteAllTextAsync(Path.ChangeExtension(output, "path"), fullSource);
        return (output, null);
    }

    public bool TryGetPath(string source, [NotNullWhen(true)] out string? cachedPath)
    {
        var key = Path.GetFullPath(source).GetStableHashCode();
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
