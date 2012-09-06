using Legend.Models;

namespace Legend.Commands
{
    public class CallerContext
    {
        public string ClientId { get; set; }
        public Reference<Player> PlayerReference { get; set; }
    }
}