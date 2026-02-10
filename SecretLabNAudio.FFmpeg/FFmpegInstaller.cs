using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine.Networking;
using Logger = LabApi.Features.Console.Logger;

namespace SecretLabNAudio.FFmpeg;

public sealed partial class FFmpegInstaller
{

    private const string Folder = "SLNA-ffmpeg";

    private const string LinuxUrl = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-linux64-lgpl.tar.xz";
    private const string LinuxFileName = "ffmpeg.tar.xz";
    private const string Decompress = $"tar -xf {LinuxFileName} --strip=2 --overwrite --wildcards */bin/*";
    private const string Bash = "/usr/bin/bash";

    public static bool IsInstalled
    {
        get
        {
            using var process = FFmpegSL.StartRaw("-version");
            return process?.Stdout.ReadLine()?.StartsWith("ffmpeg version ") ?? false;
        }
    }

    public static async Awaitable Install()
    {
        await Awaitable.BackgroundThreadAsync();
        Directory.CreateDirectory(Folder);
        if (PlatformInfo.singleton.IsLinux)
            await InstallLinux();
        else if (PlatformInfo.singleton.IsWindows)
            await Task.CompletedTask; // TODO
        else
        {
            Logger.Error("OS not supported");
            return;
        }

        var path = Path.Combine(Folder, "ffmpeg");
        FFmpegSL.Path = path;
        FFmpegPlugin.Instance?.Config?.Path = path;
        FFmpegPlugin.Instance?.SaveConfig();
    }

    private static async Awaitable InstallLinux()
    {
        Logger.Info("Downloading FFmpeg from BtbN builds...");
        await Download(LinuxUrl, Path.Combine(Folder, LinuxFileName));
        Logger.Info("Extracting FFmpeg...");
        var (decompressed, tarError) = await Execute(Decompress);
        if (!decompressed)
        {
            Logger.Error($"Failed to extract FFmpeg: {tarError ?? "could not start \"tar\" process"}");
            return;
        }

        Logger.Info("Making FFmpeg executable...");
        var (executed, chmodError) = await Execute("chmod +x ffmpeg");
        if (executed)
            Logger.Info("FFmpeg installed successfully");
        else
            Logger.Error($"Failed to make FFmpeg executable: {chmodError ?? "Could not start \"chmod\" process (how?)"}");
    }

    private static async Task<(bool Success, string? Error)> Execute(string command)
    {
        using var process = Process.Start(new ProcessStartInfo(Bash)
        {
            Arguments = $"-c \"{command}\"",
            RedirectStandardError = true,
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

    private static async Task Download(string url, string filename)
    {
        using var request = UnityWebRequest.Get(url);
        request.downloadHandler = new DownloadHandlerFile(filename);
        await request.SendWebRequest();
    }

}
