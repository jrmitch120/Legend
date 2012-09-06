using System.Collections.Generic;
using Legend.Models;

namespace Legend.World
{
    public interface IWorld
    {

        IWorldSettings Settings { get; }
        
        void Move(Player player, char direction);
        void Teleport(Player player, Reference<Room> roomReference);

        Player DisconnectClient(string clientId);
        Player GetPlayer(Reference<Player> playerReference);
        Player Login(string userName, string clientId);

        Room GetRoom(Reference<Room> roomId);

        CompleteRoom GetCompleteRoom(Reference<Room> roomReference);

        ICollection<Player> GetOnlinePlayers();
        ICollection<Player> GetOnlinePlayers(Reference<Room> room);
    }
}