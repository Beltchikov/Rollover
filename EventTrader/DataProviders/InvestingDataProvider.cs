using System.Collections.Generic;
using System.Windows;

namespace Dsmn.DataProviders
{
    public class InvestingDataProvider : IInvestingDataProvider
    {
        public List<string> ExpectedEps(List<string> tickerList)
        {
            // TODO
            MessageBox.Show("ExpectedEps");
            return tickerList;
        }
    }
}
