using Eomn.Ib;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eomn.DataProviders
{
    public class TwsProvider : ProviderBase, ITwsProvider
    {
        private IIbHost _ibHost;

        public TwsProvider(IIbHost ibHost, IIbClientQueue queue)
        {
            _ibHost = ibHost;
        }

        public async Task<List<string>> GetRoe(List<string> tickerList, int timeout)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                TriggerStatus($"Retrieving ROE for {ticker} {cnt++}/{tickerList.Count}");
                var conId = await _ibHost.RequestContractIdAsync(ticker, timeout);
                result.Add($"{ticker} {conId}");
            }

            return result;
        }
    }
}
