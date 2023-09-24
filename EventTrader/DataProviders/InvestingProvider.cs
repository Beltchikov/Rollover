using System.Collections.Generic;
using System.Windows;

namespace Dsmn.DataProviders
{
    public class InvestingProvider : IInvestingProvider
    {
        public List<string> GetEarningsData(string htmlSource)
        {
            var result = new List<string>();

            MessageBox.Show("GetEarningsData");

            return result;

        }
    }
}
