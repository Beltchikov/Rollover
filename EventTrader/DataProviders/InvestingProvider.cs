using System;
using System.Collections.Generic;
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

            foreach ( var tableRow in tableRows ) 
            { 
                if(!tableRow.Value.Contains("(") || !tableRow.Value.Contains(")"))
                {
                    continue;
                }
                var tableColumns= tableRow.Descendants("td").ToList();
                
                string? ticker = GetTicker(tableColumns);

                var epsForecast = tableColumns[3].Value;
                var marketCap = tableColumns[6].Value;
            }

            MessageBox.Show("GetEarningsData");

            return result;

        }

        private static string? GetTicker(List<XElement> tableColumns)
        {
            var tickerElement = tableColumns.Where(x => x.Attributes("class").First().Value.Contains("earnCalCompany")).FirstOrDefault();
            var elementValue = tickerElement?.Value.Trim();
            var ticker = elementValue?.Substring(elementValue.IndexOf("(") +1, elementValue.Length - elementValue.IndexOf(")") +3);
            return ticker;
        }
    }
}
