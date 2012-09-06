using System;
using Legend.Models;

namespace Legend.Commands
{
    [CommandAttribute("who","Player count information")]
    public class WhoCommand : PlayerCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Player callingPlayer, string[] args)
        {
            context.NotificationService.ToPlayer(callingPlayer,
                                                 new PlayerMessages(
                                                     String.Format("There are currently {0} wizards online.",
                                                                   context.World.GetOnlinePlayers().Count), MessageType.Notification));
        }
    }
}