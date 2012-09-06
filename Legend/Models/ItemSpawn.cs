using System;
using System.ComponentModel.DataAnnotations;

namespace Legend.Models
{
    public class ItemSpawn
    {
        public Reference<Item> ItemRef { get; set; } 

        [Range(0,100)]
        public int Rate { get; set; }
    }
}