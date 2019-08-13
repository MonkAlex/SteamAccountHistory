using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Newtonsoft.Json;

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
    private readonly string packages;

    internal string UserLogin { get; private set; }

    public async Task<IReadOnlyCollection<Package>> FillPackages(List<Package> packages)
    {
      const string packageId = "package_info_print";
      const string quit = "quit";
      const string mainLvl = "main";

      var file = await File.ReadAllLinesAsync(this.packages);

      var printStarted = false;
      var currentPackageContent = new List<string>();
      foreach (var line in file)
      {
        if (string.IsNullOrWhiteSpace(UserLogin) && line.StartsWith("Logging in user"))
          UserLogin = Regex.Match(line, "Logging in user '(.*?)' to Steam Public", RegexOptions.Compiled).Groups[1].Value;

        if (line.StartsWith(packageId))
          printStarted = true;
        if (line == quit)
        {
          printStarted = false;
          FillPackageApps(packages, currentPackageContent);
        }

        if (printStarted)
        {
          if (line.StartsWith(packageId))
          {
            var id = Regex.Match(line, "^package_info_print\\s+(\\d+)$");
            if (id.Success)
            {
              FillPackageApps(packages, currentPackageContent);
              currentPackageContent = new List<string>();
            }
          }
          else
          {
            currentPackageContent.Add(line);
          }
        }
      }


      return packages;
    }

    private static void FillPackageApps(List<Package> packages, List<string> currentPackageContent)
    {
      if (currentPackageContent.Any())
      {
        var vdf = string.Join(Environment.NewLine, currentPackageContent);
        var vProperty = VdfConvert.Deserialize(vdf);
        var jProperty = vProperty.ToJson().Children().Single().ToString();
        var parsedPackage = JsonConvert.DeserializeObject<GeneratedPackage>(jProperty);

        var package = packages.Single(p => p.Id == parsedPackage.PackageId);
        package.Apps.IdList = parsedPackage.Appids;
        package.Apps.FullParsed = (int)package.Apps.Total == package.Apps.IdList.Count;
      }
    }

    public PackageInfoPrintCommandParser(string packages)
    {
      this.packages = packages;
    }
  }
}
