using System;
using System.Collections.Generic;

namespace Legend.Commands.Spells
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    [Obsolete("Was used for testing.  Keeping around for now")]
    public sealed class ReagentsAttribute : Attribute
    {
        public IEnumerable<string> ReagentIds { get; set; }

        public ReagentsAttribute(params string[] itemIds)
        {
            ReagentIds = itemIds;
        }
    }
}