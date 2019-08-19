using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamAccountHistory
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var licenseParser = new LicensesPrintCommandParser("licenses.txt");
      var packages = await licenseParser.GetPackages();
      packages = packages.OrderBy(p => p.State.Purchased).ToList();

      var packageInfoParser = new PackageInfoPrintCommandParser("packages.txt");
      await packageInfoParser.GenerateScript(packages, licenseParser.UserLogin);
      await packageInfoParser.FillPackages(packages);

      var appInfoParser = new AppInfoPrintCommandParser("apps.txt");
      await appInfoParser.GenerateScript(packages, licenseParser.UserLogin);
      await appInfoParser.FillPackages(packages);

      var apps = packages.SelectMany(p => p.Apps.List).Distinct().GroupBy(a => a.Type);
      foreach (var app in apps)
      {
        Console.WriteLine(app.Key);
      }
    }
  }
}
