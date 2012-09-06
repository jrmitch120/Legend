using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Legend.Infrastructure;

namespace Legend.Models
{
    public class Room : IGameObject
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Hint { get; set; }
        public string Name { get; set; }

        public DateTime LastAccessed { get; set; }

        public ICollection<Reference<Player>> PlayerReferences { get; set; }
        public ICollection<Item> Items { get; set; }
        
        public IDictionary<char, Reference<Room>> AdjacentRoomReferences { get; set; }

        public Spawns Spawns { get; set; }

        public Room()
        {
            PlayerReferences = new SafeCollection<Reference<Player>>();
            Items = new SafeCollection<Item>();
            Spawns = new Spawns();
            AdjacentRoomReferences = new ConcurrentDictionary<char, Reference<Room>>();
        }
    }

    
}