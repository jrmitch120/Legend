using System;
using Legend.Commands.Spells;
using Legend.Models;

namespace Legend.Commands
{
    [CommandAttribute("cast","Cast a spell")]
    public class CastCommand : PlayerCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Player callingPlayer, string[] args)
        {
            if (args.Length == 0 || String.IsNullOrWhiteSpace(args[0]))
                throw new InvalidOperationException("...What would you like to cast?");

            var sm = new SpellManager(callingPlayer, context.World, context.NotificationService);
            sm.TryHandleSpell(args);
        }
    }
}