using System;

namespace Legend.Commands.Spells
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SpellAttribute : Attribute
    {
        public string SpellName { get; private set; }
        public string Description { get; set; }

        public SpellAttribute(string spellName, string description)
        {
            SpellName = spellName;
            Description = description;
        }
    }
}