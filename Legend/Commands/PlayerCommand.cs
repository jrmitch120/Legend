using System;
using Legend.Models;

namespace Legend.Commands
{
    /// <summary>
    /// Base class for basic game commands that require a player
    /// </summary>
    public abstract class PlayerCommand : ICommand
    {
        void ICommand.Execute(CommandContext context, CallerContext callerContext, string[] args)
        {
            Player player = context.World.GetPlayer(callerContext.PlayerReference);

            player.LastCommand = DateTime.Now;

            Execute(context, callerContext, player, args);
        }

        public abstract void Execute(CommandContext context, CallerContext callerContext, Player callingPlayer, string[] args);
    }
}