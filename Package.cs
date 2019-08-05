using System;
using System.Text.RegularExpressions;

namespace SteamAccountHistory
{
  public class Package
  {
    public ulong Id { get; set; }

    public State State { get; set; }

    public Apps Apps { get; set; }

    public Depots Depots { get; set; }

    public static Package Parse(string line)
    {
      var match = Regex.Match(line, "^License packageID\\s+(\\d+):$", RegexOptions.Compiled);
      if (!match.Success)
        throw new Exception($"parse {line} as {typeof(Package)} failed.");

      return new Package()
      {
        Id = ulong.Parse(match.Groups[1].Value)
      };
    }
  }
}