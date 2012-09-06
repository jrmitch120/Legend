using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Legend.Models;

namespace Legend.Extensions
{
    
    public static class LanguageExtensions
    {
        public static void AddRange<T, TS>(this Dictionary<T, TS> source, Dictionary<T, TS> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
                
                // Skip key dupes
            }
        }

        public static bool IsNullOrEmpty(this Array collection)
        {
            return collection == null || collection.Length == 0;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

    public static class PlayerExtensions
    {
        public static Player FindByName(this IEnumerable<Player> players, string targetName)
        {
            return (players.FirstOrDefault(x => x.Name.StartsWith(targetName, StringComparison.OrdinalIgnoreCase)));
        }

        public static IEnumerable<Player> ExcludePlayer(this IEnumerable<Player> players, Player excludePlayer)
        {
            return (ExcludePlayers(players, new[] { excludePlayer }));
        }
        
        public static IEnumerable<Player> ExcludePlayers(this IEnumerable<Player> players, IEnumerable<Player> excludePlayers)
        {
            return (players.Except(excludePlayers));
        }
    }

    public static class DisplayExtensions
    {
        private static string ApplyArticle(this string targetString)
        {
            targetString = targetString.ToLower();
            if (targetString.ToLower()[0] == 'a' || targetString.ToLower()[0] == 'e' ||
                targetString.ToLower()[0] == 'i' || targetString.ToLower()[0] == 'o' || targetString.ToLower()[0] == 'u')
                return (String.Format("an {0}",targetString));

            return (String.Format("a {0}", targetString));
        }

        public static string ToDisplay(this IGameObject gameObj)
        {
            return (ToDisplay(new[] {gameObj}));
        }

        public static string ToDisplay(this IEnumerable<IGameObject> gameObjs, bool preappendArticles = true)
        {
            var listGameObjs = gameObjs.OrderBy(x => x.Name).ToList();
            var sb = new StringBuilder();

            if (listGameObjs.Count == 0)
                return (String.Empty);

            if (listGameObjs.Count == 1)
                sb.AppendFormat(preappendArticles ? listGameObjs[0].Name.ApplyArticle() : listGameObjs[0].Name);
            else if (listGameObjs.Count == 2)
            {
                sb.AppendFormat("{0} and ",
                                preappendArticles ? listGameObjs[0].Name.ApplyArticle() : listGameObjs[0].Name);

                sb.Append(preappendArticles ? listGameObjs[1].Name.ApplyArticle() : listGameObjs[1].Name);

            }
            else
            {
                for (int i = 0; i < listGameObjs.Count; i++)
                {
                    sb.AppendFormat("{0}",
                                    preappendArticles ? listGameObjs[i].Name.ApplyArticle() : listGameObjs[i].Name);

                    if (i + 2 < listGameObjs.Count)
                        sb.Append(", ");
                    else if (i + 1 < listGameObjs.Count)
                        sb.Append(", and ");
                }
            }

            return (sb.ToString());
        }

        //public static string ToDisplay(this GameObject gameObj)
        //{
        //    return (String.Format("{0} {1}", GetArticle(gameObj.Name), gameObj.Name));
        //}

        public static string ToDisplay(this IEnumerable<Player> players)
        {
            var playerList = players.OrderBy(x => x.Name).ToList();
            if (playerList.Count == 0)
                return (String.Empty);
            if(playerList.Count == 1)
                return(String.Format("{0} is here.",playerList[0].Name));
            if(playerList.Count == 2)
                return (String.Format("{0} and {1} are here.", playerList[0].Name, playerList[1].Name));

            return (String.Format("{0} are here.", playerList.ToDisplay(false)));
        }

        public static string ToDirection(this char direction)
        {
            switch(direction.ToString().ToLower())
            {
                case "n":
                    return "north";
                case "s":
                    return "south";
                case "e":
                    return "east";
                case "w":
                    return "west";
                default:
                    return (string.Empty);
            }
        }

        public static string ToOppositeDirection(this char direction)
        {
            switch (direction.ToString().ToLower())
            {
                case "n":
                    return "south";
                case "s":
                    return "north";
                case "e":
                    return "west";
                case "w":
                    return "east";
                default:
                    return (string.Empty);
            }
        }
    }
}