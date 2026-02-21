using System.Diagnostics;
using System.Threading.Tasks;
using Logger = LabApi.Features.Console.Logger;

namespace SecretLabNAudio.FFmpeg.Installer;

public static partial class FFmpegInstaller
{

    private const string LinuxUrl = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-linux64-lgpl.tar.xz";

    private const string LinuxFileName = "ffmpeg.tar.xz";

    private const string Decompress = $"tar -xf {LinuxFileName} --strip=2 --overwrite --wildcards */bin/*";

    private const string Bash = "/usr/bin/bash";

    private static async Awaitable<string?> InstallLinux()
    {
        Logger.Info("Downloading FFmpeg from BtbN builds...");
        await Download(LinuxUrl, Path.Combine(Folder, LinuxFileName));
        Logger.Info("Extracting FFmpeg...");
        var (decompressed, tarError) = await ExecuteBash(Decompress);
        if (!decompressed)
        {
            Logger.Error($"Failed to extract FFmpeg:\n{tarError ?? "could not start \"tar\" process via bash"}");
            return null;
        }

        Logger.Info("FFmpeg installed successfully");
        return Path.Combine(Folder, "ffmpeg");
    }

    private static async Task<(bool Success, string? Error)> ExecuteBash(string command)
    {
        using var process = Process.Start(new ProcessStartInfo(Bash)
        {
            Arguments = $"-c \"{command}\"",
            RedirectStandardError = true,
            UseShellExecute = false,
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
