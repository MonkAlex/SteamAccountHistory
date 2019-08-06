using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamAccountHistory
{
  public class CommandParser
  {
    
  }

  public class LicensesPrintCommandParser : CommandParser
  {
    private readonly string licenses;

    internal string UserLogin { get; private set; }

    public async Task<IReadOnlyCollection<Package>> GetPackages()
    {
      const string packageId = "License packageID";
      const string packagePartPrefix = " - ";
      const string statePrefix = " - State";
      const string appsPrefix = " - Apps";
      const string depotsPrefix = " - Depots";

      var file = await File.ReadAllLinesAsync(this.licenses);

      List<Package> packages = new List<Package>();
      foreach (var line in file)
      {
        if (line.StartsWith("Logging in user"))
          UserLogin = Regex.Match(line, "Logging in user '(.*?)' to Steam Public", RegexOptions.Compiled).Groups[1].Value;

        if (!string.IsNullOrWhiteSpace(UserLogin) && (line.StartsWith(packageId) || (line.StartsWith(packagePartPrefix))))
        {
          if (line.StartsWith(packageId))
            packages.Add(Package.Parse(line));

          else if (line.StartsWith(statePrefix))
            packages.Last().State = State.Parse(line);

          else if (line.StartsWith(appsPrefix))
            packages.Last().Apps = Apps.Parse<Apps>(line);

          else if (line.StartsWith(depotsPrefix))
            packages.Last().Depots = Depots.Parse<Depots>(line);

          else
            throw new Exception($"Line {line} not parsed.");
        }
        else
        {
          Console.WriteLine(line);
        }
      }

      return packages;
    }

    public LicensesPrintCommandParser(string licenses)
    {
      this.licenses = licenses;
    }
  }

  public class PackageInfoPrintCommandParser : CommandParser
  {

  }
}
