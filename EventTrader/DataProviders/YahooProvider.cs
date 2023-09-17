using Dsmn.WebScraping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;

namespace Dsmn.DataProviders
{
    public class YahooProvider : IYahooProvider
    {
        IBrowserWrapper _browserWrapper;
        XmlNamespaceManager _xmlNamespaceManager;
        string urlTemplate= $"https://finance.yahoo.com/quote/TICKER/analysis?p=TICKER";

        public YahooProvider(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
            _xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            _xmlNamespaceManager.AddNamespace("empty", "http://bel.com/2023/bel-schema");
}
        public List<string> ExpectedEps(List<string> tickerList)
        {
            var result = new List<string>();
            
            foreach (string ticker in tickerList)
            {
                var url = urlTemplate.Replace("TICKER", ticker);

                if (!_browserWrapper.Navigate(url))
                {
                    throw new ApplicationException($"Can not navigate to {url}");
                }

                var xDocument = _browserWrapper.XDocument;
                var text = _browserWrapper.CurrentHtml;
                var lines = text.Split("\r\n").ToList();
                var line = lines.FirstOrDefault(l => l.Contains("Avg. Estimate"));
                var line2 = line?.Substring(line.IndexOf("<tbody>"), line.IndexOf("</tbody>") - line.IndexOf("<tbody>"));
                var line3 = line2?.Substring(line2.IndexOf("Avg. Estimate"));
                var line4 = line3?.Substring(line3.IndexOf("<td class=\"Ta(end)\">"));

                var pattern1 = @"\d[\.\d]+";
                var rx = new Regex(pattern1, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var matchCollection1 = rx.Matches(line4).ToList();
                var epsExpected = matchCollection1[0];

                //MessageBox.Show($"{ticker}: {epsExpected}");
                result.Add($"{ticker}\t{epsExpected}");
            }



           

            //xPathEpsExpected = "//a";
            //xPathEpsExpected = @"//div";
            //xPathEpsExpected = "//a[.='B']";
            //var xElementActual = xDocument.XPathSelectElement(xPathEpsExpected, _xmlNamespaceManager)
                            //?? throw new ApplicationException($"Can not find XPath element: {xPathEpsExpected}");


            //TODO
            
            return result;
        }
    }
}
