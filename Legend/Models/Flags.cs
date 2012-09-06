using System;

namespace Legend.Models
{
    [Flags]
    public enum PlayerFlags : ulong
    {
        Invisible = 1,
        Refelection = 1 << 1,
        FireShield = 1 << 2,
    }

    [Flags]
    public enum QuestFlags : ulong
    {
        Birthstone1 = 1,
        Birthstone2 = 1 << 1,
        Birthstone3 = 1 << 2,
        Birthstone4 = 1 << 3,
    }
}