using CommandSystem;

namespace SecretLabNAudio.FFmpeg.Installer;

[CommandHandler(typeof(GameConsoleCommandHandler))]
internal sealed class ChmodCommand : ICommand
{

    public string Command => "chmodFFmpeg";
    public string[] Aliases { get; } = [];
    public string Description => "Makes FFmpeg executable (Linux only)";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        (var success, response) = FFmpegInstaller.MakeExecutable();
        return success;
    }

}
