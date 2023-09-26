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

            TriggerStatus($"Retrieving ROE for {"SKX"} {tickerList.Count}");
            //var conId = await _ibHost.RequestContractIdAsync("SKX", timeout);

            //result.Add($"SKX\t{conId}");   

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                TriggerStatus($"Retrieving bid/ask spread for {ticker} {cnt++}/{tickerList.Count}");
                var conId = await _ibHost.RequestContractIdAsync(ticker, timeout);


                // Exchange must be empty
                //ibClient.ClientSocket.reqSecDefOptParams(_activeReqId, symbol, exchange, secType, conId);

                //ibClient.SecurityDefinitionOptionParameter += OnSecurityDefinitionOptionParameter;
                //ibClient.SecurityDefinitionOptionParameterEnd += OnSecurityDefinitionOptionParameterEnd;

                // TODO
                result.Add($"{ticker} {conId}");

            }

            return result;
        }
    }
}
