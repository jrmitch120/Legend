using Legend.Services;
using Legend.World;

namespace Legend.Commands
{
    public class CommandContextBase
    {
        public IWorld World { get; set; }
        public IWorldService WorldService { get; set; }
        public INotificationService NotificationService { get; set; }
    }
}