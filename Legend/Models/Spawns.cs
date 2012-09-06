using System;
using System.Collections.Generic;
using Legend.Infrastructure;

namespace Legend.Models
{
    public class Spawns
    {
        public DateTime LastItemRun { get; set; }

        public ICollection<ItemSpawn> ItemSpawns { get; set; }

        public Spawns()
        {
            ItemSpawns = new SafeCollection<ItemSpawn>();
        }
    }
}