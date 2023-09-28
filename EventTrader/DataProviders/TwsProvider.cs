using Eomn.Ib;
using IBApi;
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

        public List<string> ExtractIdsFromContractDetailsList(List<ContractDetails> contractDetailsList)
        {
            TriggerStatus($"Extracting contract ids from the contract details");
            var result = new List<string>();

            foreach (ContractDetails contractDetails in contractDetailsList)
            {
               result.Add($"{contractDetails?.Contract.LocalSymbol} {contractDetails?.Contract.ConId}");
            }

            return result;
        }

        public async Task<IEnumerable<string>> GetFundamentalData(List<string> tickerList, string reportType, int timeout)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                var tickerTrimmed = ticker.Trim();
                TriggerStatus($"Retrieving fundamental data for {tickerTrimmed}, report type: {reportType} {cnt++}/{tickerList.Count}");
                
                var fundamentalDataString = await _ibHost.RequestFundamentalDataAsync(tickerTrimmed, reportType, timeout);
                
                result.Add($"{tickerTrimmed} {fundamentalDataString[..10]}");
            }

            return result;
        }
    }
}
