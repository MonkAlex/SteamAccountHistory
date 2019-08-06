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

      var notParsedAllApps = packages.Where(p => !p.Apps.FullParsed).ToList();

      var packageBuilder = new StringBuilder();
      packageBuilder.AppendLine("@NoPromptForPassword 1");
      packageBuilder.AppendLine($"login {licenseParser.UserLogin}");
      foreach (var package in notParsedAllApps)
      {
        packageBuilder.AppendLine($"package_info_print {package.Id}");
      }

      packageBuilder.AppendLine("quit");
      File.WriteAllText("get_packages.rsc", packageBuilder.ToString());
    }
  }
}
