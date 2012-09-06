using Legend.Models;

namespace Legend.Commands.Spells
{
    [Spell("teleport","Teleport from one location to another.")]
    public class TeleportSpell : SpellCommand
    {
        public override void Execute(SpellContext context, Player callingPlayer, string[] args)
        {
                context.NotificationService.ToPlayer(callingPlayer, new PlayerMessages("Got your teleport spell request."));
        }
    }
}