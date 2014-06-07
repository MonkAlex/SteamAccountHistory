using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SteamAccountHistory
{
    public class SteamDB
    {
        private static WebClient Client;

        public enum AppType
        {
            Application,
            Config,
            Demo,
            DLC,
            Game,
            Guide,
            Tool,
            Unknown
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        private static WebClient GetClient()
        {
            if (Client == null)
                Client = new WebClient();
            Client.Headers[HttpRequestHeader.UserAgent] = "Monk";
            return Client;
        }

        public static IEnumerable<SteamApp> GetOnlyGames(string[] allPackages)
        {
            var games = Serializer<List<SteamApp>>.Load(".\\games") ?? new List<SteamApp>(Enumerable.Empty<SteamApp>());
            foreach (var packageGames in allPackages.Select(GetGames).Where(packageGames => packageGames != null))
                games.AddRange(packageGames);
            Serializer<List<SteamApp>>.Save(".\\games", games);
            games = games.Distinct().ToList();
            Serializer<List<SteamApp>>.Save(".\\distinct_games", games);

            return games;
        }

        public static IEnumerable<SteamApp> GetGames(string packageName)
        {
            var apps = new List<SteamApp>();
            var doc = new HtmlDocument();
            doc.LoadHtml(GetPackagePage(packageName));
            var type = doc.DocumentNode.SelectNodes("//table[@id=\"table-apps\"]//td");

            if (type == null)
                return null;

            for (var i = 0; i < type.Count; i += 4)
            {
                var subId = Convert.ToInt32(type[i].InnerText);
                var subType = ParseEnum<AppType>(type[i + 1].InnerText);
                var subName = type[i + 2].InnerText;
                apps.Add(new SteamApp { Type = subType, AppId = subId, PackageName = packageName, Name = subName });
            }

            return apps.Where(a => a.Type == AppType.Game);
        }

        private static string GetPackagePage(string packageName)
        {
            var searchPage = GetClient().DownloadString("http://steamdb.info/search/?a=sub&q=" + packageName.Replace(" ", "+"));
            var doc = new HtmlDocument();
            doc.LoadHtml(searchPage);
            var subIdNode = doc.DocumentNode.SelectSingleNode("//table[@id=\"table-sortable\"]//a");
            return subIdNode == null ?
                string.Empty :
                GetClient().DownloadString("http://steamdb.info/sub/" + subIdNode.InnerText + "/apps/");
        }

    }
}
