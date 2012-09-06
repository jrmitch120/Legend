using System;

namespace Legend.Models
{
    public interface IGameObject
    {
        string Id { get; set; }
        string Name { get; set; }
     
        DateTime LastAccessed { get; set; }
    }
}