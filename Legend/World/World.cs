using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Legend.Models;
using Legend.Services;

namespace Legend.World
{
    public class World : IWorld
    {
        public IWorldSettings Settings { get; private set; }

        private static Timer _ticker;
        private readonly IWorldService _service;

        private readonly ConcurrentDictionary<Reference<Room>, Room> _rooms;
        private readonly ConcurrentDictionary<Reference<Player>, Player> _players;

        private readonly ConcurrentDictionary<Reference<IGameObject>, DateTime> _saveHistory;

        private DateTime _lastSpawn = DateTime.Now;
        private readonly TimeSpan _spawnEvery = new TimeSpan(12, 0, 0);

        public World(IWorldService service)
        {
            Settings = new WorldSettings();
            
            _service = service;

            _rooms = new ConcurrentDictionary<Reference<Room>, Room>();
            _players = new ConcurrentDictionary<Reference<Player>, Player>();
            _saveHistory = new ConcurrentDictionary<Reference<IGameObject>, DateTime>();

            _ticker = new Timer(Tick, this, 5, 10000);
        }

        private static void Tick(Object world)
        {
            ((World)world).PlayerTick();
            ((World)world).CheckSpawn();
            ((World)world).Save();
        }

        private void CheckSpawn()
        {
            if(DateTime.Now.Subtract(_lastSpawn) > _spawnEvery)
                _lastSpawn = DateTime.Now;
        }

        private void Spawn(Room room)
        {
            if (room.Spawns.LastItemRun > _lastSpawn || room.PlayerReferences.Count > 0)
                return;

            var rand = new Random();

            var itemIds =
                from itemSpawn in room.Spawns.ItemSpawns
                where rand.Next(1, 101) <= itemSpawn.Rate
                select itemSpawn.ItemRef.Id;

            room.Spawns.LastItemRun = DateTime.Now;
            room.Items.Clear();
            _service.GetGameObjectsById<Item>(itemIds).ToList().ForEach(x => room.Items.Add(x.Clone()));
        }

        private void PlayerTick()
        {
            foreach(Player player in _players.Values)
            {
                    if (player.CurrentMp < player.MaxMp) // Mp regen
                        player.CurrentMp++;
            }
        }

        public void Move(Player player, char direction)
        {
            var currentRoom = GetRoom(player.RoomReference);

            if (!currentRoom.AdjacentRoomReferences.ContainsKey(direction))
                throw new InvalidOperationException("You can't go that way!");
            
            var newRoom = GetRoom(currentRoom.AdjacentRoomReferences[direction]);

            player.RoomReference = newRoom;

            currentRoom.PlayerReferences.Remove(player);
            newRoom.PlayerReferences.Add(player);
        }

        public void Save()
        {
            DateTime lastSave;
            var gameObjs = new List<IGameObject>();
            
            // Rooms
            foreach(Room room in _rooms.Values)
            {
                _saveHistory.TryGetValue(room, out lastSave);
                if (lastSave == default(DateTime) || lastSave < room.LastAccessed)
                {
                    _saveHistory.AddOrUpdate(room, DateTime.Now, (key, oldValue) => DateTime.Now);
                    gameObjs.Add(room);
                }
            }

            // Players
            foreach (Player player in _players.Values)
            {
                _saveHistory.TryGetValue(player, out lastSave);
                if (lastSave == default(DateTime) || lastSave < player.LastAccessed)
                {
                    _saveHistory.AddOrUpdate(player, DateTime.Now, (key, oldValue) => DateTime.Now);
                    gameObjs.Add(player);
                }
            }

            _service.Save(gameObjs);
        }

        public void Teleport(Player player, Reference<Room> roomReference)
        {
            var currentRoom = GetRoom(player.RoomReference);
            var newRoom = GetRoom(roomReference);

            player.RoomReference = newRoom;

            currentRoom.PlayerReferences.Remove(player);
            newRoom.PlayerReferences.Add(player);
        }

        public Player DisconnectClient(string clientId)
        {
            var player = _players.Values.FirstOrDefault(x => x.ClientIds.Contains(clientId));

            if (player == null)
                return null;

            player.ClientIds.Remove(clientId);
            if (player.ClientIds.Count == 0)
            {
                GetRoom(player.RoomReference).PlayerReferences.Remove(player);
                _service.Save(player);

                _players.TryRemove(player, out player);
            }

            return (player);
        }

        public Player Login(string playerName, string clientId)
        {
            var player = _service.GetPlayerByName(playerName);
            
            if (player == null)
                throw new InvalidOperationException(String.Format("'{0}' isn't anybody I know.", playerName));

            if(!player.Online)
                player.Online = true;

            player.ClientIds.Add(clientId);

            GetRoom(player.RoomReference).PlayerReferences.Add(player);

            _players.AddOrUpdate(player, player, (key, oldValue) => player);
            
            return (player);
        }

        public Player GetPlayer(Reference<Player> playerReference)
        {
            Player player;

            if (!_players.TryGetValue(playerReference, out player))
            {
                player = _service.GetGameObjectById<Player>(playerReference.Id);

                if (player == null)
                    return (null);
                
                _players.TryAdd(player, player);
            }
            
            player.LastAccessed = DateTime.Now;

            return (player);
        }

        public Room GetRoom(Reference<Room> roomReference)
        {
            var room = _rooms.GetOrAdd(roomReference, r => _service.GetGameObjectById<Room>(roomReference.Id));
            Spawn(room);
            room.LastAccessed = DateTime.Now;

            return (room);
        }

        public CompleteRoom GetCompleteRoom(Reference<Room> roomReference)
        {
            var room = GetRoom(roomReference);
            var players = GetOnlinePlayers(room);

            return (new CompleteRoom(room, players));
        }

        public ICollection<Player> GetOnlinePlayers(Reference<Room> room)
        {
            return
                (_players.Where(x => x.Value.Online && x.Value.RoomReference.Id == room.Id).Select(x => x.Value).ToList());
        }

        public ICollection<Player> GetOnlinePlayers()
        {
            return (_players.Where(x => x.Value.Online).Select(x => x.Value).ToList());
        }
    }
}