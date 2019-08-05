using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SteamAccountHistory
{
  public class State
  {
    public string Status { get; set; }
    public string Flags { get; set; }
    public DateTime Purchased { get; set; }
    public string Country { get; set; }
    public string PurchaseType { get; set; }

    public static State Parse(string line)
    {
      var match = Regex.Match(line, "^\\s-\\sState\\s*:\\s(.*?)\\s*\\((.*?)\\)\\s-\\sPurchased\\s:\\s(.*?)\\sin\\s\"(.*?)\",\\s(.*?)$", RegexOptions.Compiled);
      if (!match.Success)
        throw new Exception($"parse {line} as {typeof(State)} failed.");

      return new State()
      {
        Status = match.Groups[1].Value,
        Flags = match.Groups[2].Value,
        Purchased = DateTime.ParseExact(match.Groups[3].Value, "ddd MMM d HH:mm:ss yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces),
        Country = match.Groups[4].Value,
        PurchaseType = match.Groups[5].Value
      };
    }
  }
}