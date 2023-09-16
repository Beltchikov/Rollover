using Dsmn.WebScraping;
using System.Collections.Generic;
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
        public List<string> ExpectedEps(List<string> tickerList)
        {
            //foreach (string ticker in tickerList)
            //{

            //}
            
            
            //TODO
            MessageBox.Show("ExpectedEps");
            return tickerList;
        }
    }
}
