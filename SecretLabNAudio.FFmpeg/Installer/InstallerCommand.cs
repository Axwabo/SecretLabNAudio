using CommandSystem;

namespace SecretLabNAudio.FFmpeg.Installer;

[CommandHandler(typeof(GameConsoleCommandHandler))]
internal sealed class InstallerCommand : ICommand, IUsageProvider
{

    public string Command => "installFFmpeg";
    public string[] Aliases { get; } = [];
    public string Description => "Installs FFmpeg";
    public string[] Usage { get; } = ["[force]"];

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (FFmpegInstaller.IsInstallationInProgress)
        {
            response = "Installation is already in progress.";
            return false;
        }

        // java moment
        if ((arguments.Count == 0 || !"force".Equals(arguments.At(0), StringComparison.OrdinalIgnoreCase))
            && FFmpegInstaller.IsInstalled)
        {
            response = "FFmpeg is already installed.";
            return false;
        }

        _ = FFmpegInstaller.Install();
        response = "Installation started.";
        return true;
    }

}
