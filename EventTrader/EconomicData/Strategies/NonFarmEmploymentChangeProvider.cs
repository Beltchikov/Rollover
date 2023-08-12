using EventTrader.WebScraping;
using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace EventTrader.EconomicData.Strategies
{
    public class NonFarmEmploymentChangeProvider : IEconomicDataProvider
    {
        IBrowserWrapper _browserWrapper;
        XmlNamespaceManager _xmlNamespaceManager;

        public NonFarmEmploymentChangeProvider(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
            _xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
            _xmlNamespaceManager.AddNamespace("empty", "http://bel.com/2023/bel-schema");
        }


        public (double?, double?, double?) GetData(
            string url,
            string xPathActual,
            string xPathExpected,
            string xPathPrevious,
            string nullPlaceholder)
        {
            // TODO remove later
            url = "https://www.investing.com/economic-calendar/";
            xPathActual = "//*[@id=\"eventActual_479390\"]";
            xPathExpected = "//*[@id=\"eventForecast_479390\"]";
            xPathPrevious = "//*[@id=\"eventPrevious_479390\"]";
            nullPlaceholder = "&nbsp;";


            if (!_browserWrapper.Navigate(url))
            {
                throw new ApplicationException($"Can not navigate to {url}");
            }

            var xDocument = _browserWrapper.XDocument;

            double? actual = GetDoubleValueByXPath(xDocument, xPathActual, nullPlaceholder);
            double? expected = GetDoubleValueByXPath(xDocument, xPathExpected, nullPlaceholder);
            double? previous = GetDoubleValueByXPath(xDocument, xPathPrevious, nullPlaceholder);

            return (actual, expected, previous);
        }

        private double? GetDoubleValueByXPath(XDocument xDocument, string xPath, string nullPlaceholder)
        {
            double? actual;
            var xElementActual = xDocument.XPathSelectElement(xPath, _xmlNamespaceManager)
                            ?? throw new ApplicationException($"Can not find XPath element: {xPath}");
            var stringValueActual = xElementActual.Value.Trim()
                ?? throw new ApplicationException($"The value of the XElement is null.");
            actual = ParseDouble(stringValueActual, nullPlaceholder, ParseOptions.RemoveLastCharacter);
            return actual;
        }

        private double? ParseDouble(string doubleAsText, string nullPlaceholder, ParseOptions parseOptions)
        {
            if (doubleAsText.Trim().ToLower() == nullPlaceholder.Trim().ToLower())
            {
                return null;
            }

            string preProcessed = doubleAsText;
            if (parseOptions == ParseOptions.RemoveLastCharacter)
            {
                preProcessed = preProcessed[..^1];
            }

            if (!double.TryParse(preProcessed, NumberStyles.Currency, CultureInfo.InvariantCulture, out double doubleValue))
            {
                throw new ApplicationException($"Can not parse {doubleAsText} into double.");
            }
            return doubleValue;
        }
    }
}
