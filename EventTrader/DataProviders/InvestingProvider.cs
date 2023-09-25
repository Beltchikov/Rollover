using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace Dsmn.DataProviders
{
    public class InvestingProvider : IInvestingProvider
    {
        XmlNamespaceManager _xmlNamespaceManager;

        public InvestingProvider()
        {
            _xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            _xmlNamespaceManager.AddNamespace("empty", "http://bel.com/2023/bel-schema");
        }

        public List<string> GetEarningsData(string htmlSource)
        {
            var result = new List<string>();

            htmlSource = htmlSource.Replace("/&nbsp;", "");
            htmlSource = htmlSource.Replace("&nbsp;", "");
            var xDocument = XDocument.Parse(htmlSource);
            var tableRows = xDocument.Descendants("tr").ToList();

            foreach (var tableRow in tableRows)
            {
                if (!tableRow.Value.Contains("(") || !tableRow.Value.Contains(")"))
                {
                    continue;
                }
                var tableColumns = tableRow.Descendants("td").ToList();

                string? ticker = GetTicker(tableColumns);
                string? epsForecast = GetEpsForecast(tableColumns);
                string? marketCap = GetMarketCap(tableColumns);

                result.Add($"{ticker}\t{marketCap}\t{epsForecast}");
            }

            return result;

        }

        private string? GetMarketCap(List<XElement> tableColumns)
        {
            var marketCap = tableColumns[6].Value;

            if (marketCap != null && marketCap.EndsWith("M"))
            {
                marketCap = marketCap[..^1];
                var marketCapAsDouble = Double.Parse(marketCap, new CultureInfo("EN-US"));
                marketCapAsDouble = marketCapAsDouble / 1000;
                marketCapAsDouble = Math.Round(marketCapAsDouble, 3);
                marketCap = marketCapAsDouble.ToString(CultureInfo.InvariantCulture);
            }
            if (marketCap != null && marketCap.EndsWith("B"))
            {
                marketCap = marketCap[..^1];
            }

            return marketCap;
        }

        private string? GetEpsForecast(List<XElement> tableColumns)
        {
            var epsForecast = tableColumns[3].Value;
            return epsForecast;
        }

        private static string? GetTicker(List<XElement> tableColumns)
        {
            var tickerElement = tableColumns.Where(x => x.Attributes("class").First().Value.Contains("earnCalCompany")).FirstOrDefault();
            var elementValue = tickerElement?.Value.Trim();

            int? i1 = elementValue?.IndexOf("(") + 1;
            int? i2 = elementValue?.IndexOf(")");

            if (i1 == null || i2 == null)
            {
                return null;
            }

            var ticker = elementValue?.Substring(i1.Value, i2.Value - i1.Value);
            return ticker;
        }
    }
}
