using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Legend.Infrastructure;

namespace Legend.Models
{
    public class Player : IGameObject
    {
        public Reference<Room> RoomReference { get; set; }

        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public bool BriefDescriptions { get; set; }
        public bool IsAdmin { get; set; }
        public bool Online { get; set; }

        public int CurrentHp { get; set; }
        public int CurrentMp { get; set; }
        public int Gold { get; set; }
        public int Level { get; set; }
        public int MaxHp { get; set; }
        public int MaxMp { get; set; }

        public PlayerFlags PlayerStatus { get; set; }

        public DateTime LastAccessed { get; set; }
        public DateTime LastCommand { get; set; }

        public ICollection<string> ClientIds { get; set; }
        public ICollection<Item> Items { get; set; }
        public IDictionary<string, Spell> Spells { get; set; }

        public Player()
        {
            ClientIds = new SafeCollection<string>();
            Items = new SafeCollection<Item>();

            // need this because serialization doesn't store case insensitivity
            Spells = new CaseInsensitiveConcurrentDictionary<Spell>();

            /*
            PlayerStatus = PlayerStatus | PlayerStatuses.Invisible;
            var chk1 = PlayerStatus.HasFlag(PlayerStatuses.Invisible);
            PlayerStatus = PlayerStatus ^ PlayerStatuses.Invisible;
            var chk2 = PlayerStatus.HasFlag(PlayerStatuses.Invisible);
            */
            Reset();
        }

        public void Reset()
        {
            RoomReference = new Reference<Room> { Id = "Rooms/1" };

            Items.Clear();
            Spells.Clear();

            Level = 1;
            Gold = 0;
            CurrentHp = MaxHp = 10;
            CurrentMp = MaxMp = 10;

            PlayerStatus = new PlayerFlags();
        }

        public bool IsDead(int damage)
        {
            CurrentHp -= damage;
            if (CurrentHp <= 0)
                return (true);

            return (false);
        }
    }
}