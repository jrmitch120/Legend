using System.Collections.Concurrent;
using System.Collections.Generic;
using Legend.Infrastructure;

namespace Legend.Models
{
    public class CompleteRoom
    {
        public ICollection<Player> Players { get; set; }
        public Room Room { get; set; }

        public CompleteRoom()
        {
        }

        public CompleteRoom(Room room, ICollection<Player> players)
        {
            Room = room;
            Players = players;
        }
    }
}