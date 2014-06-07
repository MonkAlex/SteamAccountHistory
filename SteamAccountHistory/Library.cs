using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SteamAccountHistory
{
    class Library
    {

        private static string path = ".\\games";

        public static List<SteamApp> AllApps = Load();

        public static SteamApp.AppStatus FilterStatus = SteamApp.AppStatus.Active | SteamApp.AppStatus.Planned;
        public static List<SteamApp> VisibleGames
        {
            get
            {
                return AllGames
                    .Where(g => FilterStatus.HasFlag(g.Status))
                    .ToList();
            }
        }

        public static List<SteamApp> AllGames
        {
            get
            {
                return AllApps
                    .Where(g => g.Type == SteamDB.AppType.Game)
                    .ToList();
            }
        }

        public static void Save()
        {
            Serializer<List<SteamApp>>.Save(path, AllApps);
        }

        public static List<SteamApp> Load()
        {
            return AllApps ?? (Serializer<List<SteamApp>>.Load(path) ?? new List<SteamApp>(Enumerable.Empty<SteamApp>()));
        }

        public static void ChangeStatus(SteamApp.AppStatus status, IList apps)
        {
            if (apps == null)
                return;

            foreach (var app in apps.OfType<SteamApp>())
            {
                app.Status = status;
            }
        }
    }
}
