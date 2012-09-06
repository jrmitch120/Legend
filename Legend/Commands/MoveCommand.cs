using System;
using Legend.Extensions;
using Legend.Models;

namespace Legend.Commands
{
    [CommandAttribute("n", "Move north")]
    [CommandAttribute("s", "Move south")]
    [CommandAttribute("e", "Move east")]
    [CommandAttribute("w","Move west")]
    public class MoveCommand : PlayerCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Player callingPlayer, string[] args)
        {
            var direction = context.CommandName[0];
            var leavingRoom = callingPlayer.RoomReference;

            context.World.Move(callingPlayer, direction);

            context.NotificationService.LeaveRoom(callingPlayer, leavingRoom,
                                                  new PlayerMessages(String.Format("...{0} leaves to the {1}.",
                                                                                   callingPlayer.Name,
                                                                                   direction.ToDirection())));

            context.NotificationService.EnterRoom(callingPlayer,
                                                  new PlayerMessages(String.Format("...{0} arrives from the {1}.",
                                                                                   callingPlayer.Name,
                                                                                   direction.ToOppositeDirection())));

            var lookCmd = new LookCommand();
            lookCmd.Execute(context, callerContext, callingPlayer, null);
        }
    }
}