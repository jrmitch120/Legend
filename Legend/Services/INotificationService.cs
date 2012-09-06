using System.Collections.Generic;
using Legend.Models;

namespace Legend.Services
{
    public interface INotificationService
    {
        void Die(Player player);

        void EnterRoom(Player player, PlayerMessages messages);
        void LeaveRoom(Player player, Reference<Room> room, PlayerMessages messages);
        
        void ToAll(PlayerMessages messages);
        void ToPlayer(Player player, PlayerMessages messages);
        void ToPlayers(IEnumerable<Player> players, PlayerMessages messages);
        void ToRoom(Room room, PlayerMessages messages);
    }
}