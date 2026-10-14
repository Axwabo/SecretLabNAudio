using UnityEngine.Networking;

namespace SecretLabNAudio.FFmpeg.Installer;

/// <summary>Helper class to install FFmpeg.</summary>
public static partial class FFmpegInstaller
{

    /// <summary>The directory where downloaded files are placed.</summary>
    public const string Folder = "SLNA-ffmpeg";

    private const string Success = "FFmpeg installed successfully";

    /// <summary>Whether an installation process is already pending.</summary>
    public static bool IsInstallationInProgress { get; private set; }

    /// <summary>Checks whether FFmpeg is installed by querying the version and reading from its standard output.</summary>
    /// <remarks>This method may take some time to execute.</remarks>
    public static bool IsInstalled()
    {
        using var process = FFmpegSL.Start("-version");
        process?.WaitForExit();
        return process?.Stdout.ReadLine()?.StartsWith("ffmpeg version ") ?? false;
    }

    /// <summary>
    /// Attempts to find FFmpeg on disk, and if not found, asynchronously downloads and installs it.
    /// </summary>
    /// <param name="forceRefresh">Whether to download and install FFmpeg even if it can be found in the <see cref="Folder"/> or the <c>ffmpeg</c> directory.</param>
    /// <returns>An <see cref="Awaitable"/> indicating installation success.</returns>
    /// <remarks>The configuration is overwritten if the installation succeeds.</remarks>
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
        catch (UnityHttpException e)
        {
            Logger.Error($"FFmpeg installation failed due to a network error:\n{e.Message}");
            throw;
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
        OverwriteConfig(path);
        return true;
    }

    internal static void OverwriteConfig(string path)
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

    private static async Awaitable Download(string url, string filename)
    {
        await Awaitable.MainThreadAsync();
        using var request = UnityWebRequest.Get(url);
        using var cts = new CancellationTokenSource();
        request.downloadHandler = new DownloadHandlerFile(Path.GetFullPath(filename));
        _ = LogProgress(request, cts.Token);
        await request.SendWebRequest();
        cts.Cancel();
        if (request.error is { } error)
            throw new UnityHttpException(error);
    }

    private static async Awaitable LogProgress(UnityWebRequest request, CancellationToken cancellationToken)
    {
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

    private static (bool Success, string? Error) Execute(string shell, string arguments, string workingDirectory = Folder)
    {
        using var process = Process.Start(new ProcessStartInfo(shell)
        {
            Arguments = arguments,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDirectory
        });
        if (process == null)
            return (false, null);
        process.WaitForExit();
        return process.ExitCode == 0
            ? (true, null)
            : (false, process.StandardError.ReadToEnd());
    }

}
