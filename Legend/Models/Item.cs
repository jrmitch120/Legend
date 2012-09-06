using System;

namespace Legend.Models
{
    public class Item : IGameObject, ICloneable
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }

        public DateTime LastAccessed { get; set; }
       
        object ICloneable.Clone()
        {
            return MemberwiseClone();
        }
        
        public Item Clone()
        {
            return (Item)MemberwiseClone();
        }
    }
}