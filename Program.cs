using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamAccountHistory
{
  class Program
  {
    static async Task Main(string[] args)
    {
      const string PackageStart = "License packageID";
      const string PackagePartPrefix = " - ";
      const string State = " - State";
      const string Apps = " - Apps";
      const string Depots = " - Depots";
      var file = await File.ReadAllLinesAsync("licenses.txt");
      var user = string.Empty;

      List<Package> packages = new List<Package>();
      for (var index = 0; index < file.Length; index++)
      {
        var line = file[index];
        if (line.StartsWith("Logging in user"))
          user = Regex.Match(line, "Logging in user '(.*?)' to Steam Public", RegexOptions.Compiled).Groups[1].Value;

        if (!string.IsNullOrWhiteSpace(user) && (line.StartsWith(PackageStart) || (line.StartsWith(PackagePartPrefix))))
        {
          if (line.StartsWith(PackageStart))
            packages.Add(Package.Parse(line));

          else if (line.StartsWith(State))
            packages.Last().State = SteamAccountHistory.State.Parse(line);

          else if (line.StartsWith(Apps))
            packages.Last().Apps = SteamAccountHistory.Apps.Parse<Apps>(line);

          else if (line.StartsWith(Depots))
            packages.Last().Depots = SteamAccountHistory.Depots.Parse<Depots>(line);

          else
            throw new Exception($"Line {line} not parsed.");
        }
        else
        {
          Console.WriteLine(line);
        }
      }

      packages = packages.OrderBy(p => p.State.Purchased).ToList();
      foreach (var package in packages)
      {
        Console.WriteLine(package.State.Purchased);
      }
    }
  }
}
