using System.ComponentModel.Composition;

namespace Legend.Commands
{
    [InheritedExport]
    public interface ICommand
    {
        void Execute(CommandContext context, CallerContext callerContext, string[] args);
    }
}