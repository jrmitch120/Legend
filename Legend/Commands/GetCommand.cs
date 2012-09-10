using System;
using System.Linq;
using Legend.Extensions;
using Legend.Models;

namespace Legend.Commands
{
    [CommandAttribute("get","Get an item in the current room")]
    public class GetCommand : PlayerCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Player callingPlayer, string[] args)
        {
            if (args.Length == 0 || String.IsNullOrWhiteSpace(args[0]))
                throw new InvalidOperationException("...What would you like to get?");

            if(callingPlayer.Items.Count > 5)
            {
                context.NotificationService.ToPlayer(callingPlayer, new PlayerMessages("...You're pack is too full to accomidate anything else."));
                return;
            }

            var room = context.World.GetRoom(callingPlayer.RoomReference);
            var players = context.World.GetOnlinePlayers(room).Where(player => player.Name != callingPlayer.Name).ToList();
            var item = room.Items.FindByName(String.Join(" ", args).Trim());
            
            if(item != null)
            {
                room.Items.Remove(item);
                callingPlayer.Items.Add(item);
                context.NotificationService.ToPlayers(players, 
                    new PlayerMessages(String.Format("{0} grabs {1}.", callingPlayer.Name, item.ToDisplay())));
                context.NotificationService.ToPlayer(callingPlayer,
                    new PlayerMessages(String.Format("...You grab {0}.", item.ToDisplay())));
            }
            else
                context.NotificationService.ToPlayer(callingPlayer, new PlayerMessages("...There's nothing like that around here."));
        }
    }
}