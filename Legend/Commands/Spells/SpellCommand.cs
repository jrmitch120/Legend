using System;
using Legend.Models;

namespace Legend.Commands.Spells
{
    /// <summary>
    /// Base class for spell commands that require a player
    /// </summary>
    public abstract class SpellCommand : ISpellCast
    {
        public abstract void Execute(SpellContext context, Player callingPlayer, string[] args);
    }
}