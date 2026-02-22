namespace SecretLabNAudio.FFmpeg.Installer;

public static partial class FFmpegInstaller
{

    private const string WindowsUrl = "https://www.gyan.dev/ffmpeg/builds/ffmpeg-release-essentials.zip";
    private const string WindowsArchive = "ffmpeg.zip";

    private const string ExtractWindowsTemplate = "[Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.ZipFile');"
                                                  + "$zip = [IO.Compression.ZipFile]::OpenRead('{0}');"
                                                  + "$zip.Entries | ? Name -like ffmpeg.exe"
                                                  + "| % {{ [IO.Compression.ZipFileExtensions]::ExtractToFile($_, '{1}', $true) }};"
                                                  + "$zip.Dispose()";

    private const string WindowsExecutable = "ffmpeg.exe";

    private static async Awaitable<string?> InstallWindows()
    {
        Logger.Info("Downloading FFmpeg from gyan.dev...");
        var archivePath = Path.Combine(Folder, WindowsArchive);
        var exePath = Path.Combine(Folder, WindowsExecutable);
        await Download(WindowsUrl, archivePath);
        Logger.Info("Extracting FFmpeg...");
        var command = string.Format(ExtractWindowsTemplate, Path.GetFullPath(archivePath), Path.GetFullPath(exePath));
        var (decompressed, psError) = Execute("powershell", command);
        if (!decompressed)
        {
            Logger.Info($"Failed to extract FFmpeg:\n{psError ?? "could not start PowerShell for extraction"}");
            return null;
        }

        Logger.Info(Success);
        return exePath;
    }

}
