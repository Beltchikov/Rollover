using EventTrader.WebScraping;
using System;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace EventTrader.EconomicData.Strategies
{
    public class NonFarmEmploymentChangeProvider : IEconomicDataProvider
    {
        IBrowserWrapper _browserWrapper;
        XmlNamespaceManager _xmlNamespaceManager;
        const string URL = "https://www.investing.com/economic-calendar/";
        const string XPATH = "//*[@id=\"eventRowId_479304\"]";

        public NonFarmEmploymentChangeProvider(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
            _xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            _xmlNamespaceManager.AddNamespace("empty", "http://bel.com/2023/bel-schema");
        }

        public (double?, double?, double?) GetData()
        {
            if (!_browserWrapper.Navigate(URL))
            {
                throw new ApplicationException($"Can not navigate to {URL}");
            }

            var xDocument = _browserWrapper.XDocument;

            var xElement = xDocument.XPathSelectElement(XPATH, _xmlNamespaceManager)
                ?? throw new ApplicationException($"Can not find XPath element: {XPATH}");
            var xElementValue = xElement.Value
                ?? throw new ApplicationException($"The value of the XElement is null.");
            var values = xElementValue.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            var actual = ParseDouble(values[6][..^1]);
            var expected = ParseDouble(values[7][..^1]);
            var previous = ParseDouble(values[8][..^1]);

            return (actual, expected, previous);
        }

        private double ParseDouble(string doubleAsText)
        {
            double doubleValue;
            if (!Double.TryParse(doubleAsText, NumberStyles.Currency, CultureInfo.InvariantCulture, out doubleValue))
            {
                throw new ApplicationException($"Can not parse {doubleAsText} into double.");
            }
            return doubleValue;
        }
    }
}
