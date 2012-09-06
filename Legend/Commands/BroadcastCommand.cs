using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Legend.Models;

namespace Legend.Commands
{
    [Command("broadcast", "Sends a message to all players. Only administrators can use this command.")]
    public class BroadcastCommand : AdminCommand
    {
        public override void ExecuteAdminOperation(CommandContext context, CallerContext callerContext, Player callingUser, string[] args)
        {
            string messageText = String.Join(" ", args).Trim();
            
            if (String.IsNullOrEmpty(messageText))
                throw new InvalidOperationException("What did you want to broadcast?");

            context.NotificationService.ToAll(new PlayerMessages(messageText, MessageType.Notification));
        }
    }
}