using System.Configuration;

namespace Legend.World
{
    public class WorldSettings : IWorldSettings
    {
        public int MaxItemsInRoom
        {
            get { return (int.Parse(ConfigurationManager.AppSettings["MaxItemsInRoom"])); }
        }

        public int MaxPlayers
        {
            get { return (int.Parse(ConfigurationManager.AppSettings["MaxPlayers"])); }
        } 
    }
}