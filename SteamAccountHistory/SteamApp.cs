
namespace SteamAccountHistory
{
    public class SteamApp
    {
        public string Name;
        public string PackageName;
        public int AppId;
        public SteamDB.AppType Type;

        public override bool Equals(object obj)
        {
            var item = obj as SteamApp;
            return item != null && this.Name.Equals(item.Name) && this.AppId.Equals(item.AppId);
        }

        public override int GetHashCode()
        {
            return new {Name, AppId}.GetHashCode();
        }

    }
}
