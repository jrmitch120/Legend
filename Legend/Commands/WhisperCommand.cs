using System;
using System.Collections.Generic;
using System.Linq;
using Legend.Extensions;
using Legend.Models;

namespace Legend.Commands
{
    [CommandAttribute("whisper","Private message to somebody in the room.")]
    public class WhisperCommand : PlayerCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Player callingPlayer, string[] args)
        {
            if (args.Length < 1)
                throw new InvalidOperationException("...You need to whisper to somebody!");

            if (args.Length < 2 || String.IsNullOrWhiteSpace(args[1]))
                throw new InvalidOperationException("...What would you like to whisper?");

            var room = context.World.GetRoom(callingPlayer.RoomReference);
            var players = context.World.GetOnlinePlayers(room).Where(player => player.Name != callingPlayer.Name).ToList();

            var to = players.FindByName(args[0]);
            if (to != null)
            {
                context.NotificationService.ToPlayer(to,
                                                     new PlayerMessages
                                                         (
                                                         new List<PlayerMessage>
                                                         {
                                                             new PlayerMessage("***",MessageType.Important),
                                                             new PlayerMessage(string.Format("{0} whispers: {1}",
                                                                                             callingPlayer.Name,
                                                                                             String.Join(" ",args.Skip(1))))
                                                         }));

                context.NotificationService.ToPlayer(callingPlayer,
                                                     new PlayerMessages(String.Format("...{0} hears you!", to.Name)));

                context.NotificationService.ToPlayers(players.Where(x => x.Name != to.Name),
                                                      new PlayerMessages(String.Format(
                                                          "{0} whispers something to {1}.", callingPlayer.Name, to.Name)));
            }
            else
                context.NotificationService.ToPlayer(callingPlayer,
                                                     new PlayerMessages("...You don't see anybody here by that name."));


        }
    }
}