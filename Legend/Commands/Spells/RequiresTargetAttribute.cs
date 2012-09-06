using System;

namespace Legend.Commands.Spells
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class RequiresTargetAttribute : Attribute
    {

    }
}