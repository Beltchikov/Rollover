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
        const string URL = "https://www.investing.com/economic-calendar/";
        const string XPATH_ACTUAL = "//*[@id=\"eventActual_479390\"]";
        const string XPATH_EXPECTED = "//*[@id=\"eventForecast_479390\"]";
        const string XPATH_PREVIOUS = "//*[@id=\"eventPrevious_479390\"]";
        const string NULL_PLACEHOLDER = "&nbsp;";

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

            double? actual = GetDoubleValueByXPath(xDocument, XPATH_ACTUAL);
            double? expected = GetDoubleValueByXPath(xDocument, XPATH_EXPECTED);
            double? previous = GetDoubleValueByXPath(xDocument, XPATH_PREVIOUS);

            return (actual, expected, previous);
        }

        private double? GetDoubleValueByXPath(XDocument xDocument, string xPath)
        {
            double? actual;
            var xElementActual = xDocument.XPathSelectElement(xPath, _xmlNamespaceManager)
                            ?? throw new ApplicationException($"Can not find XPath element: {xPath}");
            var stringValueActual = xElementActual.Value.Trim()
                ?? throw new ApplicationException($"The value of the XElement is null.");
            actual = ParseDouble(stringValueActual, NULL_PLACEHOLDER, ParseOptions.RemoveLastCharacter);
            return actual;
        }

        private double? ParseDouble(string doubleAsText, string nullPlaceholder, ParseOptions parseOptions)
        {
            if(doubleAsText.Trim().ToLower() == nullPlaceholder.Trim().ToLower())
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
