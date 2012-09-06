using System;
using Legend.Extensions;
using Legend.Models;

namespace Legend.Commands.Spells
{
    [Spell("fireball","Cast a flaming ball of fire")]
    [RequiresTarget]
    public class FireballSpell : SpellCommand
    {
        public override void Execute(SpellContext context, Player callingPlayer, string[] args)
        {
            var completeRoom = context.World.GetCompleteRoom(callingPlayer.RoomReference);
            var target = completeRoom.Players.FindByName(String.Join(" ", args[0]));
            var bystanders = completeRoom.Players.ExcludePlayers(new []{callingPlayer, target});
            
            if(target == null)
            {
                context.NotificationService.ToPlayer(callingPlayer, new PlayerMessages("...The spell fizzles."));
                context.NotificationService.ToPlayers(completeRoom.Players.ExcludePlayer(callingPlayer),
                                                      new PlayerMessages(String.Format(
                                                          "{0} attempts to cast a spell, but fails miserably.",
                                                          callingPlayer.Name), MessageType.Important));
            }
            else
            {
                context.NotificationService.ToPlayers(bystanders,
                                                      new PlayerMessages(String.Format(
                                                          "A gigantic ball of flame erupts from {0}'s hands and strikes {1} full on!",
                                                          callingPlayer.Name, target.Name), MessageType.Attack));

                context.NotificationService.ToPlayer(target,
                                                     new PlayerMessages(
                                                         String.Format(
                                                             "A gigantic ball of flame erupts from {0}'s hands and strikes you in the chest!",
                                                             callingPlayer.Name), MessageType.Attack));

                context.NotificationService.ToPlayer(callingPlayer,
                                                      new PlayerMessages(String.Format(
                                                          "A gigantic ball of flame leaps from your hands and strikes {0} full on!",
                                                          target.Name), MessageType.Attack));
                
                if (target.IsDead(15))
                    context.NotificationService.Die(target);
            }
        }
    }
}