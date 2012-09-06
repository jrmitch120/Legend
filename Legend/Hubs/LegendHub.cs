using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Caching;
using Legend.Commands;
using Legend.Models;
using Legend.Services;
using Legend.ViewModels;
using Legend.World;
using SignalR.Hubs;

namespace Legend.Hubs
{
    [HubName("legendHub")]
    public class LegendHub : Hub, IDisconnect, INotificationService
    {
        private readonly IWorld _world;
        private readonly ICache _cache;

        //public LegendHub()
        //{
        //}

        public LegendHub(ICache cache, IWorld world)
        {
            _cache = cache;
            _world = world;
        }

        private void TimeOut(String key, object value, CacheItemRemovedReason removedReason)
        {
            var playerClient = (PlayerClient)value;

            Clients[playerClient.ClientId].disconnect("Connection terminated due to inactivity.");           
        }

        public bool Command(string command)
        {
            var playerClient = (PlayerClient)_cache.Get(Context.ConnectionId);

            if (playerClient == null)
                throw new InvalidOperationException("Your player session has expired.  Please refresh your browser");

            var cmd = new CommandManager(Context.ConnectionId, playerClient.PlayerReference, _world, this);
            return (cmd.TryHandleCommand(command));
        }

        public bool Login(string playerName)
        {
            var player = _world.Login(playerName, Context.ConnectionId);

            // Make a grand entrance if this is the first client.
            if(player.ClientIds.Count == 1)
                Clients[player.RoomReference.Id].receive(
                    new PlayerPacket(
                         new PlayerMessages(
                             String.Format("...You hear a slight rumbling. Suddenly, {0} coalesces out of thin air!", player.Name)
                    )));

            Groups.Add(Context.ConnectionId, player.RoomReference.Id).Wait();

            // Set the client expiration
            _cache.Set(Context.ConnectionId, new PlayerClient
            {
                PlayerReference = player, ClientId = Context.ConnectionId
            },
            TimeSpan.FromMinutes(10), TimeOut);

            return (true);
        }

        public Task Disconnect()
        {
            DisconnectClient(Context.ConnectionId);
            return null;
        }

        private void DisconnectClient(string clientId)
        {
            Player player = _world.DisconnectClient(clientId);

            if (player != null && !player.Online)
            {
                Clients[player.RoomReference.Id].receive(
                    new PlayerPacket(
                        new PlayerMessages(
                            String.Format("...{0} vanishes in a puff of blue smoke!", player.Name)
                    )));
                Groups.Remove(Context.ConnectionId, player.RoomReference.Id).Wait();
            }
        }

        void INotificationService.Die(Player player)
        {
            foreach (var clientId in player.ClientIds)
            {
                Clients[clientId].receive(new PlayerPacket(new PlayerMessages("You have died...", MessageType.Important)));
                Groups.Remove(clientId, player.RoomReference.Id).Wait();
            }

            // Let everybody know about the death.
            Clients.receive(new PlayerPacket(new PlayerMessages(String.Format("{0} has died.", player.Name), MessageType.Notification)));

            _world.Teleport(player, player.RoomReference);
            player.Reset();

            // The rebirth!
            Clients[player.RoomReference.Id].receive(
                new PlayerPacket(new PlayerMessages(String.Format("{0} is birthed in an orange light!", player.Name),
                                                    MessageType.Important)));

            // Join room 1
            foreach (var clientId in player.ClientIds)
                Groups.Add(clientId, player.RoomReference.Id).Wait();

            // TODO:  Temporary.  Add a javascript .death function to call.
            var cmd = new CommandManager(Context.ConnectionId, player, _world, this);
            cmd.TryHandleCommand("look");
        }

        void INotificationService.EnterRoom(Player player, PlayerMessages messages)
        {
            Clients[player.RoomReference.Id].receive(new PlayerPacket(messages));
            
            foreach (var clientId in player.ClientIds)
            {
                // Add to the new room
                Groups.Add(clientId, player.RoomReference.Id).Wait();
            }
        }

        void INotificationService.LeaveRoom(Player player, Reference<Room> room, PlayerMessages messages)
        {
            foreach (var clientId in player.ClientIds)
            {
                // Leave the old room
                Groups.Remove(clientId, room.Id).Wait();
            }

            Clients[room.Id].receive(new PlayerPacket(messages));
        }

        void INotificationService.ToAll(PlayerMessages messages)
        {
            Clients.receive(new PlayerPacket(messages));
        }

        void INotificationService.ToPlayer(Player player, PlayerMessages messages)
        {
            var packet = new PlayerPacket(messages);
            
            // Transmit to all user's connected clients
            foreach (var clientId in player.ClientIds)
                Clients[clientId].receive(packet);
        }

        void INotificationService.ToPlayers(IEnumerable<Player> players, PlayerMessages messages)
        {
            var packet = new PlayerPacket(messages);

            foreach (var player in players)
            {
                foreach (var clientId in player.ClientIds)
                    Clients[clientId].receive(packet);
            }
        }

        void INotificationService.ToRoom(Room room, PlayerMessages messages)
        {
            Clients[room.Id].receive(new PlayerPacket(messages));
        }
    }
}