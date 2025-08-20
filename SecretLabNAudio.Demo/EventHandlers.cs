using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using SecretLabNAudio.Demo.Board;

namespace SecretLabNAudio.Demo;

internal sealed class EventHandlers : CustomEventsHandler
{

    public override void OnServerWaitingForPlayers() => DiscJockeyBoard.SetUpStage();

    public override void OnServerCommandExecuting(CommandExecutingEventArgs ev) => CommandHandler.OnServerCommandExecuting(ev);

}
