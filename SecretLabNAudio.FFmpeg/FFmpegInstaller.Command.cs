using CommandSystem;

namespace SecretLabNAudio.FFmpeg;

[CommandHandler(typeof(GameConsoleCommandHandler))]
public sealed partial class FFmpegInstaller : ICommand
{

    public string Command => "installFFmpeg";
    public string[] Aliases { get; } = [];
    public string Description => "Installs FFmpeg";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (IsInstalled)
        {
            response = "FFmpeg is already installed.";
            return false;
        }

        _ = Install();
        response = "Installation started.";
        return true;
    }

}
