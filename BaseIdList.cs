using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SteamAccountHistory
{
  public abstract class BaseIdList
  {
    public List<ulong> IdList = new List<ulong>();

    public ulong Total { get; set; }

    public bool FullParsed { get; set; }

    public static T Parse<T>(string line) where T : BaseIdList, new()
    {
      var match = Regex.Match(line, "^\\s+-\\s+\\w+\\s+:\\s*(.*?) (|\\.\\.\\.) \\((\\d+) in total\\)$", RegexOptions.Compiled);
      if (!match.Success)
        throw new Exception($"parse {line} as {typeof(T)} failed.");

      return new T()
      {
        Total = ulong.Parse(match.Groups[3].Value),
        FullParsed = string.IsNullOrWhiteSpace(match.Groups[2].Value),
        IdList = match.Groups[1].Value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => ulong.Parse((string) t.Trim())).ToList()
      };
    }
  }
}