using Dsmn.WebScraping;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Windows;
using System.Xml.XPath;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;

namespace Dsmn.DataProviders
{
    public class InvestingDataProvider : IInvestingDataProvider
    {
        IBrowserWrapper _browserWrapper;
        XmlNamespaceManager _xmlNamespaceManager;

        public InvestingDataProvider(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
            _xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            _xmlNamespaceManager.AddNamespace("empty", "http://bel.com/2023/bel-schema");
}
        public List<string> ExpectedEps(
            string urlEpsExpected,
            string xPathEpsExpected,
            string nullPlaceholderEpsExpected,
            List<string> tickerList)
        {
            //foreach (string ticker in tickerList)
            //{

            //}

            urlEpsExpected = "https://finance.yahoo.com/quote/SKX/analysis?p=SKX";

            if (!_browserWrapper.Navigate(urlEpsExpected))
            {
                throw new ApplicationException($"Can not navigate to {urlEpsExpected}");
            }

            var xDocument = _browserWrapper.XDocument;
            var text = _browserWrapper.CurrentHtml;
            var lines = text.Split("\r\n").ToList();
            var line = lines.FirstOrDefault(l => l.Contains("Avg. Estimate"));
            var line2 = line.Substring(line.IndexOf("<tbody>"), line.IndexOf("</tbody>") - line.IndexOf("<tbody>"));
            var line3 = line2.Substring(line2.IndexOf("Avg. Estimate"));
            var line4 = line3.Substring(line3.IndexOf("<td class=\"Ta(end)\">"));

            var pattern1 = @"\d[\.\d]+";
            var rx = new Regex(pattern1, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matchCollection1 = rx.Matches(line4).ToList();
            var epsExpected = matchCollection1[0];

            //xPathEpsExpected = "//a";
            //xPathEpsExpected = @"//div";
            //xPathEpsExpected = "//a[.='B']";
            //var xElementActual = xDocument.XPathSelectElement(xPathEpsExpected, _xmlNamespaceManager)
                            //?? throw new ApplicationException($"Can not find XPath element: {xPathEpsExpected}");


            //TODO
            MessageBox.Show($"SKX: {epsExpected}");
            return tickerList;
        }
    }
}
