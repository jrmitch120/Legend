using System;
using Legend.Models;

namespace Legend.Commands
{
    [CommandAttribute("brief","Toggle brief description mode")]
    public class BriefCommand : PlayerCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Player callingPlayer, string[] args)
        {
            if (args.Length == 0 || String.IsNullOrWhiteSpace(args[0]))
                throw new InvalidOperationException("...Would you like brief descriptions on or off?");

            if (args[0].ToLower().Trim() == "on")
            {
                callingPlayer.BriefDescriptions = true;
                context.NotificationService.ToPlayer(callingPlayer, new PlayerMessages("Ok, brief descriptions from now on."));
            }
            else
            {
                callingPlayer.BriefDescriptions = false;
                context.NotificationService.ToPlayer(callingPlayer, new PlayerMessages("Ok, full descriptions from now on."));
            }
        }
    }
}