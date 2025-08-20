using System.IO;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features.Enums;
using RemoteAdmin;
using SecretLabNAudio.Core.FileReading;

namespace SecretLabNAudio.Demo.Board;

internal static class CommandHandler
{

    public static void OnServerCommandExecuting(CommandExecutingEventArgs ev)
    {
        if (ev.CommandType != CommandType.RemoteAdmin
            || !ev.CommandName.Equals("DJ", StringComparison.InvariantCultureIgnoreCase)
            || ev.Sender is not PlayerCommandSender {ReferenceHub: var hub})
            return;
        ev.IsAllowed = false;
        var player = Player.Dictionary[hub]; // ideally I'd use Player.Get, but I won't reference CommandSystem.Core
        var path = string.Join(" ", ev.Arguments);
        if (!File.Exists(path))
        {
            ev.Reply("DJ#File does not exist!", false);
            return;
        }

        if (!TryCreateAudioReader.Stream(path, out var stream))
        {
            ev.Reply("DJ#Failed to create audio stream!", false);
            return;
        }

        if (!DiscJockeyBoard.Instance)
        {
            ev.Reply("DJ#Board is not set up!", false);
            return;
        }

        DiscJockeyBoard.Instance.Play(player, stream, Path.GetFileName(path));
        ev.Reply("DJ#Playing...", true);
    }

    private static void Reply(this CommandExecutingEventArgs ev, string text, bool success)
        => ev.Sender.RaReply(text, success, true, "");

}
