using System.Collections.Generic;
using Legend.Models;

namespace Legend.ViewModels
{
    public class PlayerPacket
    {
        public PlayerPacket(PlayerMessages messages)
        {
            Display = messages;
        }

        public PlayerMessages Display { get; set; }
    }
}