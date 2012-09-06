using System;
using System.Linq;
using Legend.Extensions;
using Legend.Models;

namespace Legend.Commands
{
    [CommandAttribute("drop","Drop an item")]
    public class DropCommand : PlayerCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Player callingPlayer, string[] args)
        {
            if (args.Length == 0 || String.IsNullOrWhiteSpace(args[0]))
                throw new InvalidOperationException("...What would you like to drop?");

            var room = context.World.GetRoom(callingPlayer.RoomReference);
            var players = context.World.GetOnlinePlayers(room).Where(player => player.Name != callingPlayer.Name).ToList();
            var item = callingPlayer.Items.FirstOrDefault(x => x.Name.StartsWith(String.Join(" ", args).Trim(),StringComparison.OrdinalIgnoreCase));
            
            if(item != null)
            {
                callingPlayer.Items.Remove(item);

                context.NotificationService.ToPlayers(players,
                    new PlayerMessages(String.Format("{0} drops {1}.", callingPlayer.Name, item.ToDisplay())));
                context.NotificationService.ToPlayer(callingPlayer,
                    new PlayerMessages(String.Format("...You drop {0}.", item.ToDisplay())));
                
                if(room.Items.Count < context.World.Settings.MaxItemsInRoom)
                    room.Items.Add(item);
                else
                    context.NotificationService.ToRoom(room,
                    new PlayerMessages(String.Format("The {0} vanishes in a brilliant flash of light!", item.Name)));
            }
            else
                context.NotificationService.ToPlayer(callingPlayer, new PlayerMessages("...You're not carrying anything like that."));
        }
    }
}