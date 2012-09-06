using System;
using Legend.Models;

namespace Legend.Commands
{
    public abstract class AdminCommand : PlayerCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Player callingUser, string[] args)
        {
            if (!callingUser.IsAdmin)
                throw new InvalidOperationException("...Huh?");

            ExecuteAdminOperation(context, callerContext, callingUser, args);
        }

        public abstract void ExecuteAdminOperation(CommandContext context, CallerContext callerContext, Player callingUser, string[] args);
    }
}