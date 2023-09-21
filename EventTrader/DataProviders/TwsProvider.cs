using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dsmn.DataProviders
{
    public class TwsProvider : ITwsProvider
    {
        public async Task<List<string>> BidAskSpread(List<string> tickerListTws, int timeout)
        {
            await Task.Run(() =>
            {
                MessageBox.Show("BidAskSpread");

            });

            //todo
            return new List<string>();
        }
    }
}
