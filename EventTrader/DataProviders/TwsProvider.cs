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

        public async Task<List<string>> GetContractDetails(List<string> tickerList, int timeout)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                TriggerStatus($"Retrieving contract details for {ticker} {cnt++}/{tickerList.Count}");
                var contractDetails = await _ibHost.RequestContractDetailsAsync(ticker, timeout);
                result.Add($"{ticker} {contractDetails?.Contract.ConId}");
            }

            return result;
        }

        public Task<IEnumerable<string>> GetRoe(object tickerStringTwsRoe, int timeout)
        {
            throw new System.NotImplementedException();
        }
    }
}
