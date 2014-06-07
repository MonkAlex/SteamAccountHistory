
using System;

namespace SteamAccountHistory
{
    public class SteamApp
    {
        public string Name;
        public string PackageName;
        public ulong AppId;
        public SteamDB.AppType Type;
        public AppStatus Status;

        [Flags]
        public enum AppStatus
        {
            Active,
            Completed,
            Planned,
            Ignored
        }

        public override string ToString()
        {
            return string.Format("{0}", Name) ;
        }

        public override bool Equals(object obj)
        {
            var item = obj as SteamApp;
            return item != null && this.AppId.Equals(item.AppId);
        }

        public override int GetHashCode()
        {
            return (AppId*42).GetHashCode();
        }

    }
}
