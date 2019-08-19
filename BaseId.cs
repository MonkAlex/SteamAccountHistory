using Newtonsoft.Json.Linq;

namespace SteamAccountHistory
{
  public abstract class BaseId
  {
    public ulong Id { get; set; }

    public JToken Body { get; set; }

    protected bool Equals(BaseId other)
    {
      return Id == other.Id;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj))
        return false;
      if (ReferenceEquals(this, obj))
        return true;
      if (obj.GetType() != this.GetType())
        return false;
      return Equals((BaseId)obj);
    }

    public override int GetHashCode()
    {
      return Id.GetHashCode();
    }

    public static bool operator ==(BaseId left, BaseId right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(BaseId left, BaseId right)
    {
      return !Equals(left, right);
    }
  }
}
