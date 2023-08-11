using EventTrader.WebScraping;
using System;
using System.Reflection.Metadata;
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
            _xmlNamespaceManager.AddNamespace("empty", "http://demo.com/2011/demo-schema");
        }

        public (double?, double?, double?) GetData()
        {
            if(! _browserWrapper.Navigate(URL))
            {
                throw new ApplicationException($"Can not navigate to {URL}");
            }

            //var htmlSource = _browserWrapper.CurrentHtml;
            var xDocument = _browserWrapper.XDocument;
            //var namespaceManager = new XmlNamespaceManager(new NameTable());
            //namespaceManager.AddNamespace("empty", "http://demo.com/2011/demo-schema");
            var htmlRowValue = xDocument.XPathSelectElement(XPATH, _xmlNamespaceManager)?.Value;
            var values = htmlRowValue?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            throw new NotImplementedException();
        }
    }
}
