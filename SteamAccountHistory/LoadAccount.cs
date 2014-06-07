using System;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace SteamAccountHistory
{
    public class LoadAccount
    {
        static string path = @"C:\Users\aLex\Documents\visual studio 2013\Projects\SteamAccountHistory\SteamAccountHistory\bin\Debug\account.htm";
        private static string page = string.Empty;

        public static string Get()
        {
            return page = (page == string.Empty) ? (File.Exists(path) ? File.ReadAllText(path) : string.Empty) : page;
        }

        public static string[] GetPackages()
        {
            Get();
            var packages = new List<string>();
            var document = new HtmlDocument();
            document.LoadHtml(page);
            var block = document.DocumentNode.SelectNodes("//div[@class=\"block_content\"]//p");
            if (block != null)
                packages.AddRange(block.Select(node => node.InnerText));

            return packages.ToArray();
        }
    }
}
