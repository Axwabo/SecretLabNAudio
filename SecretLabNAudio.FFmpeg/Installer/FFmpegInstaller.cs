using System.Threading.Tasks;
using UnityEngine.Networking;
using Logger = LabApi.Features.Console.Logger;

namespace SecretLabNAudio.FFmpeg.Installer;

public static partial class FFmpegInstaller
{

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

    public static async Awaitable<bool> Install()
    {
        Directory.CreateDirectory(Folder);
        await Awaitable.BackgroundThreadAsync();
        IsInstallationInProgress = true;
        string? path;
        try
        {
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
        var fullPath = Path.GetFullPath(Path.Combine(Folder, path));
        FFmpegSL.Path = fullPath;
        FFmpegPlugin.Instance?.Config?.Path = fullPath;
        FFmpegPlugin.Instance?.SaveConfig();
        return true;
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
        request.downloadHandler = new DownloadHandlerFile(Path.GetFullPath(filename));
        await request.SendWebRequest();
    }

}
