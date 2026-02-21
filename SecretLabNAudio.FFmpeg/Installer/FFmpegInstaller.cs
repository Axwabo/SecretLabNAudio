using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Logger = LabApi.Features.Console.Logger;

namespace SecretLabNAudio.FFmpeg.Installer;

public static partial class FFmpegInstaller
{

    private const string Success = "FFmpeg installed successfully";
    private const string Folder = "SLNA-ffmpeg";

    public static bool IsInstallationInProgress { get; private set; }

    public static bool IsInstalled
    {
        get
        {
            using var process = FFmpegSL.StartRaw("-version");
            return process?.Stdout.ReadLine()?.StartsWith("ffmpeg version ") ?? false;
        }
    }

    public static async Awaitable<bool> Install(bool forceRefresh = false)
    {
        if (IsInstallationInProgress)
            return false;
        IsInstallationInProgress = true;
        Directory.CreateDirectory(Folder);
        await Awaitable.BackgroundThreadAsync();
        string? path;
        try
        {
            if (forceRefresh || !TryCopyExisting(out path))
                path = await InstallOSSpecific();
        }
        catch (Exception e)
        {
            Logger.Error($"FFmpeg installation failed:\n{e}");
            throw;
        }
        finally
        {
            IsInstallationInProgress = false;
        }

        if (path == null)
            return false;
        OverrideConfig(path);
        return true;
    }

    internal static void OverrideConfig(string path)
    {
        var fullPath = Path.GetFullPath(path);
        FFmpegSL.Path = fullPath;
        FFmpegPlugin.Instance?.Config?.Path = fullPath;
        FFmpegPlugin.Instance?.SaveConfig();
    }

    private static async Awaitable<string?> InstallOSSpecific()
    {
        if (PlatformInfo.singleton.IsLinux)
            return await InstallLinux();
        if (PlatformInfo.singleton.IsWindows)
            return null; // TODO
        Logger.Error("Unsupported operating system");
        return null;
    }

    private static async Task Download(string url, string filename)
    {
        using var request = UnityWebRequest.Get(url);
        using var cts = new CancellationTokenSource();
        request.downloadHandler = new DownloadHandlerFile(Path.GetFullPath(filename));
        _ = LogProgress(request, cts.Token);
        await request.SendWebRequest();
    }

    private static async Awaitable LogProgress(UnityWebRequest request, CancellationToken cancellationToken)
    {
        var progress = 0f;
        while (request.result == UnityWebRequest.Result.InProgress)
        {
            var currentProgress = request.downloadProgress;
            if (!Mathf.Approximately(currentProgress, progress))
                Logger.Debug($"Download progress: {currentProgress:P}");
            progress = currentProgress;
            await Awaitable.NextFrameAsync(cancellationToken);
        }
    }

    internal static bool TryCopyExisting(out string destination)
    {
        var filename = PlatformInfo.singleton.IsWindows ? "ffmpeg.exe" : "ffmpeg";
        var source = Path.Combine(AppContext.BaseDirectory, "ffmpeg", filename);
        destination = Path.Combine(Folder, filename);
        if (!File.Exists(source) || File.Exists(destination))
            return false;
        Logger.Info("Copying existing FFmpeg installation...");
        File.Copy(source, destination);
        Logger.Info(Success);
        return true;
    }

}
