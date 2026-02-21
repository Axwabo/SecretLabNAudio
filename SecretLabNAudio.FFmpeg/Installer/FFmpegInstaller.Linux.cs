using Logger = LabApi.Features.Console.Logger;

namespace SecretLabNAudio.FFmpeg.Installer;

public static partial class FFmpegInstaller
{

    private const string LinuxUrl = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-linux64-lgpl.tar.xz";
    private const string LinuxArchive = "ffmpeg.tar.xz";
    private const string LinuxDecompress = $"tar -xf {LinuxArchive} --strip=2 --overwrite --wildcards */bin/*";
    private const string Bash = "/usr/bin/bash";
    private const string LinuxExecutable = "ffmpeg";

    private static async Awaitable<string?> InstallLinux()
    {
        Logger.Info("Downloading FFmpeg from BtbN builds...");
        await Download(LinuxUrl, Path.Combine(Folder, LinuxArchive));
        Logger.Info("Extracting FFmpeg...");
        var (decompressed, tarError) = await Execute(Bash, $"-c \"{LinuxDecompress}\"");
        if (!decompressed)
        {
            Logger.Error($"Failed to extract FFmpeg:\n{tarError ?? "could not start \"tar\" process via bash"}");
            return null;
        }

        Logger.Info(Success);
        return Path.Combine(Folder, LinuxExecutable);
    }

}
