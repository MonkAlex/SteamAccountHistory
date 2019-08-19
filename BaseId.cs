using Newtonsoft.Json.Linq;

namespace SteamAccountHistory
{
  public abstract class BaseId
  {
    public ulong Id { get; set; }

    public JToken Body { get; set; }
  }
}
