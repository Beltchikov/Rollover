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
                var tickerTrimmed = ticker.Trim();
                TriggerStatus($"Retrieving contract details for {tickerTrimmed} {cnt++}/{tickerList.Count}");
                var contractDetails = await _ibHost.RequestContractDetailsAsync(tickerTrimmed, timeout);
                result.Add($"{tickerTrimmed} {contractDetails?.Contract.ConId}");
            }

            return result;
        }

        public async Task<IEnumerable<string>> GetRoe(List<string> tickerList, int timeout)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                var tickerTrimmed = ticker.Trim();
                TriggerStatus($"Retrieving ROE for {tickerTrimmed} {cnt++}/{tickerList.Count}");
                
                var contractDetails = await _ibHost.RequestContractDetailsAsync(tickerTrimmed, timeout);
                
                //result.Add($"{tickerTrimmed} {contractDetails?.Contract.ConId}");
                result.Add($"{tickerTrimmed} TODO");
            }

            return result;
        }
    }
}
