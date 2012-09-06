using System.ComponentModel.Composition;
using Legend.Models;

namespace Legend.Commands.Spells
{
    [InheritedExport]
    public interface ISpellCast
    {
        void Execute(SpellContext context, Player player, string[] args);
    }
}