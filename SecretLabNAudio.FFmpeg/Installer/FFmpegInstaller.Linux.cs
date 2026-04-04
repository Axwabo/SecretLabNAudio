namespace SecretLabNAudio.FFmpeg.Installer;

public static partial class FFmpegInstaller
{

    private const string LinuxUrl = "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-linux64-lgpl.tar.xz";
    private const string LinuxArchive = "ffmpeg.tar.xz";
    private const string LinuxDecompress = $"tar -xf {LinuxArchive} --strip=2 --overwrite --wildcards */bin/*";
    private const string Env = "/usr/bin/env";
    private const string LinuxExecutable = "ffmpeg";

    private static async Awaitable<string?> InstallLinux()
    {
        Logger.Info("Downloading FFmpeg from BtbN builds...");
        await Download(LinuxUrl, Path.Combine(Folder, LinuxArchive));
        Logger.Info("Extracting FFmpeg...");
        var (decompressed, tarError) = Execute(Env, $"bash -c \"{LinuxDecompress}\"");
        if (!decompressed)
        {
            Logger.Error($"Failed to extract FFmpeg:\n{tarError ?? "could not start \"tar\" process via bash and env"}");
            return null;
        }

        Logger.Info(Success);
        return Path.Combine(Folder, LinuxExecutable);
    }

    internal static (bool Success, string Response) MakeExecutable()
    {
        if (!PlatformInfo.singleton.IsLinux)
            return (false, "This command is only available on Linux.");
        if (!File.Exists(FFmpegSL.Path))
            return (false, "The configured FFmpeg installation does not exist or is not a file that can be made executable.");
        var (changed, chmodError) = Execute(
            Env,
            $"chmod +x \"{Path.GetFileName(FFmpegSL.Path)}\"",
            Path.GetFullPath(Path.GetDirectoryName(FFmpegSL.Path) ?? FFmpegSL.Path)
        );
        return changed
            ? (true, "Successfully made FFmpeg executable.")
            : (false, chmodError ?? "Could not start \"chmod\" process via env");
    }

}
