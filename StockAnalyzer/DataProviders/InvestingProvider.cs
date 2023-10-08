using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace StockAnalyzer.DataProviders
{
    public class InvestingProvider : IInvestingProvider
    {
        XmlNamespaceManager _xmlNamespaceManager;

        public InvestingProvider()
        {
            _xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            _xmlNamespaceManager.AddNamespace("empty", "http://bel.com/2023/bel-schema");
        }


        public List<string> GetEarningsData(string htmlSource, double minMarketCap)
        {
            var result = new List<string>();

            htmlSource = htmlSource.Replace("/&nbsp;", "");
            htmlSource = htmlSource.Replace("&nbsp;", "");
            var xDocument = XDocument.Parse(htmlSource);
            var tableRows = xDocument.Descendants("tr").ToList();

            result.Add($"TICKER\tMarket Cap B\tEarnings\tEPS Forecast");
            DateTime earningsDateOrMinValue = DateTime.MinValue;
            foreach (var tableRow in tableRows)
            {
                if (!tableRow.Value.Contains("(") || !tableRow.Value.Contains(")"))
                {
                    if (!DateTime.TryParse(tableRow.Value, out earningsDateOrMinValue))
                    {
                        earningsDateOrMinValue = DateTime.MinValue;
                    }
                    continue;
                }
                var tableColumns = tableRow.Descendants("td").ToList();

                double marketCap = GetMarketCapAsDouble(tableColumns);
                if(marketCap < minMarketCap)
                {
                    continue;
                }

                string? ticker = GetTicker(tableColumns);
                string? epsForecast = GetEpsForecast(tableColumns);
                string? marketCapAsString = GetMarketCapAsString(tableColumns);
                string? earningsDate = earningsDateOrMinValue == DateTime.MinValue ? null : earningsDateOrMinValue.ToString("dd.MM.yyyy");

                result.Add($"{ticker}\t{marketCapAsString}\t{earningsDate}\t{epsForecast}");
            }

            return result;
        }

        private double GetMarketCapAsDouble(List<XElement> tableColumns)
        {
            var marketCap = tableColumns[6].Value;
            if(marketCap == null)
            {
                return .0;
            }

            double marketCapAsDouble = 0;
            if (marketCap != null && marketCap.EndsWith("M"))
            {
                marketCap = marketCap[..^1];
                marketCapAsDouble = double.Parse(marketCap, new CultureInfo("EN-US"));
                marketCapAsDouble = marketCapAsDouble / 1000;
                marketCapAsDouble = Math.Round(marketCapAsDouble, 3);
            }
            if (marketCap != null && marketCap.EndsWith("B"))
            {
                marketCap = marketCap[..^1];
                marketCapAsDouble = double.Parse(marketCap, new CultureInfo("EN-US"));
            }

            return marketCapAsDouble;
        }

        private string? GetMarketCapAsString(List<XElement> tableColumns)
        {
            double marketCapAsDouble = GetMarketCapAsDouble(tableColumns); 
            if(marketCapAsDouble <= 0)
            {
                return null;
            }

            string marketCap = marketCapAsDouble.ToString("0.000");
            return marketCap;
        }

        private string? GetEpsForecast(List<XElement> tableColumns)
        {
            var epsForecast = tableColumns[3].Value;
            
            double epsForecastAsDouble;
            if(!double.TryParse(epsForecast, NumberStyles.Number, new CultureInfo("EN-US"), out epsForecastAsDouble))
            {
                return null;
            }

            epsForecast = epsForecastAsDouble.ToString("0.000");
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
