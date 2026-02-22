using CommandSystem;

namespace SecretLabNAudio.FFmpeg.Installer;

[CommandHandler(typeof(GameConsoleCommandHandler))]
internal sealed class InstallCommand : ICommand, IUsageProvider
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
        var force = arguments.Count != 0 && "force".Equals(arguments.At(0), StringComparison.OrdinalIgnoreCase);
        if (!force && FFmpegInstaller.IsInstalled)
        {
            response = "FFmpeg is already installed.";
            return false;
        }

        _ = FFmpegInstaller.Install(force);
        response = "Installation started.";
        return true;
    }

}
