using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

      var file = await File.ReadAllLinesAsync(this.licenses, Encoding.UTF8);

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

    public async Task<string> GenerateScript(IReadOnlyCollection<Package> allPackages, string userLogin)
    {
      var notParsedAllApps = allPackages.Where(p => !p.Apps.FullParsed).ToList();

      var packageBuilder = new StringBuilder();
      packageBuilder.AppendLine("@NoPromptForPassword 1");
      packageBuilder.AppendLine($"login {userLogin}");
      foreach (var package in notParsedAllApps)
      {
        packageBuilder.AppendLine($"package_info_print {package.Id}");
      }

      packageBuilder.AppendLine("quit");
      var packagesRsc = "get_packages.rsc";
      await File.WriteAllTextAsync(packagesRsc, packageBuilder.ToString());
      return packagesRsc;
    }

    public async Task FillPackages(IReadOnlyCollection<Package> allPackages)
    {
      const string packageId = "package_info_print";
      const string quit = "quit";

      var file = await File.ReadAllLinesAsync(this.packages, Encoding.UTF8);

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
          FillPackageApps(allPackages, currentPackageContent);
        }

        if (printStarted)
        {
          if (line.StartsWith(packageId))
          {
            var id = Regex.Match(line, "^package_info_print\\s+(\\d+)$");
            if (id.Success)
            {
              FillPackageApps(allPackages, currentPackageContent);
              currentPackageContent = new List<string>();
            }
          }
          else
          {
            currentPackageContent.Add(line);
          }
        }
      }
    }

    private static void FillPackageApps(IReadOnlyCollection<Package> packages, List<string> currentPackageContent)
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

  public class AppInfoPrintCommandParser : CommandParser
  {
    private readonly string apps;

    internal string UserLogin { get; private set; }

    public async Task<string> GenerateScript(IReadOnlyCollection<Package> allPackages, string userLogin)
    {
      var appBuilder = new StringBuilder();
      appBuilder.AppendLine("@NoPromptForPassword 1");
      appBuilder.AppendLine($"login {userLogin}");

      var allApps = allPackages.SelectMany(p => p.Apps.IdList).Distinct().OrderBy(i => i).ToList();

      foreach (var app in allApps)
      {
        appBuilder.AppendLine($"app_info_print {app}");
      }

      appBuilder.AppendLine("quit");
      var appsRsc = "get_apps.rsc";
      await File.WriteAllTextAsync(appsRsc, appBuilder.ToString());
      return appsRsc;
    }

    public async Task FillPackages(IReadOnlyCollection<Package> allPackages)
    {
      const string appId = "app_info_print";
      const string quit = "quit";

      var file = await File.ReadAllLinesAsync(this.apps, Encoding.UTF8);

      var printStarted = false;
      var currentAppContent = new List<string>();
      foreach (var line in file)
      {
        if (string.IsNullOrWhiteSpace(UserLogin) && line.StartsWith("Logging in user"))
          UserLogin = Regex.Match(line, "Logging in user '(.*?)' to Steam Public", RegexOptions.Compiled).Groups[1].Value;

        if (line.StartsWith(appId))
          printStarted = true;
        if (line == quit)
        {
          printStarted = false;
          FillPackageApps(allPackages, currentAppContent);
        }

        if (printStarted)
        {
          if (line.StartsWith(appId))
          {
            var id = Regex.Match(line, "^app_info_print\\s+(\\d+)");
            if (id.Success)
            {
              FillPackageApps(allPackages, currentAppContent);
              currentAppContent = new List<string>();
            }
          }
          else
          {
            currentAppContent.Add(line);
          }
        }
      }
    }

    private void FillPackageApps(IReadOnlyCollection<Package> packages, List<string> currentAppContent)
    {
      if (currentAppContent.Any())
      {
        var vdf = string.Join(Environment.NewLine, currentAppContent.Skip(1));
        var vProperty = VdfConvert.Deserialize(vdf);
        var vdfJsonConversionSettings = new VdfJsonConversionSettings()
        {
          // Merge data with duplicate name. Registry or install parameters. Registry merged, install parameters can replace sometimes.
          ObjectDuplicateKeyHandling = DuplicateKeyHandling.Merge
        };
        var jProperty = vProperty.ToJson(vdfJsonConversionSettings).Children().Single();
        if (jProperty.HasValues)
        {
          FillAppsBody(packages, jProperty);
        }
      }
    }

    private static void FillAppsBody(IReadOnlyCollection<Package> packages, JToken jProperty)
    {
      var gameId = (jProperty["common"]["gameid"] as JValue).Value<ulong>();
      var findApp = packages.SelectMany(p => p.Apps.List).Where(a => a.Id == gameId).ToList();
      foreach (var app in findApp)
      {
        app.Body = jProperty;
      }
    }

    public AppInfoPrintCommandParser(string apps)
    {
      this.apps = apps;
    }
  }
}
