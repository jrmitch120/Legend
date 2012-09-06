using System;
using System.Linq;
using Legend.Extensions;
using Legend.Models;

namespace Legend.Commands
{
    [CommandAttribute("say","Say something to the people in the current room")]
    public class SayCommand : PlayerCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Player callingPlayer, string[] args)
        {
            if (args.Length == 0 || String.IsNullOrWhiteSpace(args[0]))
                throw new InvalidOperationException("...What would you like to say?");

            var room = context.World.GetRoom(callingPlayer.RoomReference);
            var players = context.World.GetOnlinePlayers(room).ExcludePlayer(callingPlayer).ToList();

            var message = String.Format("{0} says: {1}", callingPlayer.Name, String.Join(" ", args).Trim());

            if (players.Any())
                context.NotificationService.ToPlayers(players, new PlayerMessages(message));

            context.NotificationService.ToPlayer(callingPlayer, new PlayerMessages("...You said it!"));
        }
    }
}