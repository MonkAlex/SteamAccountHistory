using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SteamAccountHistory
{
  public abstract class BaseIdList<T> where T : BaseId, new()
  {
    public List<ulong> IdList = new List<ulong>();

    public List<T> List = new List<T>();

    public ulong Total { get; set; }

    public bool FullParsed { get; set; }

    public static TCollection Parse<TCollection>(string line) where TCollection : BaseIdList<T>, new()
    {
      var match = Regex.Match(line, "^\\s+-\\s+\\w+\\s+:\\s*(.*?) (|\\.\\.\\.) \\((\\d+) in total\\)$", RegexOptions.Compiled);
      if (!match.Success)
        throw new Exception($"parse {line} as {typeof(T)} failed.");

      var result = new TCollection()
      {
        Total = ulong.Parse(match.Groups[3].Value),
        FullParsed = string.IsNullOrWhiteSpace(match.Groups[2].Value),
        IdList = match.Groups[1].Value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => ulong.Parse(t.Trim())).ToList(),
      };
      result.List = result.IdList.Select(i => new T() { Id = i }).ToList();

      return result;
    }
  }
}
