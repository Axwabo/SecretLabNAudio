using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SecretLabNAudio.FFmpeg.Interop;
using UnityEngine.Networking;

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
            if (!forceRefresh && TryFindExisting(out path))
                Logger.Debug("Found FFmpeg on disk");
            else
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
            return await InstallWindows();
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
        cts.Cancel();
    }

    private static async Awaitable LogProgress(UnityWebRequest request, CancellationToken cancellationToken)
    {
        await Awaitable.MainThreadAsync();
        var progress = -1f;
        while (request.result == UnityWebRequest.Result.InProgress)
        {
            var currentProgress = request.downloadProgress;
            if (!Mathf.Approximately(currentProgress, progress))
                Logger.Debug($"{currentProgress:P}");
            progress = currentProgress;
            await Awaitable.NextFrameAsync(cancellationToken);
        }
    }

    internal static bool TryFindExisting(out string path)
    {
        var filename = PlatformInfo.singleton.IsWindows ? WindowsExecutable : LinuxExecutable;
        var primary = Path.Combine(Folder, filename);
        if (File.Exists(primary))
        {
            path = primary;
            return true;
        }

        path = Path.Combine(AppContext.BaseDirectory, "ffmpeg", filename);
        return File.Exists(path);
    }

    private static async Task<(bool Success, string? Error)> Execute(string shell, string arguments)
    {
        using var process = Process.Start(new ProcessStartInfo(shell)
        {
            Arguments = arguments,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = Folder
        });
        if (process == null)
            return (false, null);
        process.WaitForExit();
        if (process.ExitCode == 0)
            return (true, null);
        var error = await process.StandardError.ReadToEndAsync();
        return (false, error);
    }

}
