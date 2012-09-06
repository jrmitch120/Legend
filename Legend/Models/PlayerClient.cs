namespace Legend.Models
{
    public class PlayerClient
    {
        public Reference<Player> PlayerReference { get; set; }
        public string ClientId { get; set; }
    }
}