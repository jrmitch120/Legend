using System;
using Legend.Extensions;
using Legend.Models;

namespace Legend.Commands
{
    [CommandAttribute("inv","Check inventory")]
    [CommandAttribute("i", "Check inventory")]
    public class InventoryCommand : PlayerCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Player callingPlayer, string[] args)
        {

            if(callingPlayer.Items.Count == 0)
                context.NotificationService.ToPlayer(callingPlayer, 
                    new PlayerMessages("...You're not carrying anything."));
            else
                context.NotificationService.ToPlayer(callingPlayer,
                        new PlayerMessages(
                            String.Format("...You are carrying {0}.", callingPlayer.Items.ToDisplay())));
        }
    }
}