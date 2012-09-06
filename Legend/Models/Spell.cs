using System;
using System.Collections.Generic;
using Legend.Infrastructure;

namespace Legend.Models
{
    public class Spell : IGameObject
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public int MinLevel { get; set; }
        public int MpRequired { get; set; }

        public DateTime LastAccessed { get; set; }

        public ICollection<Reference<Item>> Reagents { get; set; }
        
        public Spell()
        {
            Reagents = new SafeCollection<Reference<Item>>();
            MinLevel = 0;
        }
    }
}