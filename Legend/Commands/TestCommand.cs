using System.Collections.Generic;
using Legend.Models;

namespace Legend.Commands
{
    [CommandAttribute("test","Testing 123!")]
    public class TestCommand : PlayerCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Player callingPlayer, string[] args)
        {
            //context.World.CreateWorld();

            context.NotificationService.ToPlayer(callingPlayer, new PlayerMessages("World created!"));

            //context.NotificationService.ToPlayer(callingUser);
        }
    }
}