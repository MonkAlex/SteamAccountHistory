namespace SteamAccountHistory
{
  public class App : BaseId
  {
    public string Name { get { return this.Body?["common"]?["name"]?.ToString(); } }
    public string Type { get { return this.Body?["common"]?["type"]?.ToString().ToLower(); } }
  }
}
