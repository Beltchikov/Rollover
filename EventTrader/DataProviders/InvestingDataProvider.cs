using Dsmn.WebScraping;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Windows;

namespace Dsmn.DataProviders
{
    public class InvestingDataProvider : IInvestingDataProvider
    {
        IBrowserWrapper _browserWrapper;

        public InvestingDataProvider(IBrowserWrapper browserWrapper)
        {
            _browserWrapper = browserWrapper;
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

            if (!_browserWrapper.Navigate(urlEpsExpected))
            {
                throw new ApplicationException($"Can not navigate to {urlEpsExpected}");
            }

            var xDocument = _browserWrapper.XDocument;


            //TODO
            MessageBox.Show("ExpectedEps");
            return tickerList;
        }
    }
}
