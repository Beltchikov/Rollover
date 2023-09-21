using Dsmn.Ib;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dsmn.DataProviders
{
    public class TwsProvider : ProviderBase, ITwsProvider
    {
        private IIbHost _ibHost;

        public TwsProvider(IIbHost ibHost)
        {
            _ibHost = ibHost;
        }

        public async Task<List<string>> BidAskSpread(List<string> tickerList, int timeout)
        {
            var result = new List<string>();

            int cnt = 1;
            foreach (string ticker in tickerList)
            {
                await Task.Run(() =>
                {
                    TriggerStatus($"Retrieving bid/ask spread for {ticker} {cnt++}/{tickerList.Count}");
                    var conId = _ibHost.RequestContractId(ticker, timeout);


                    // Exchange must be empty
                    //ibClient.ClientSocket.reqSecDefOptParams(_activeReqId, symbol, exchange, secType, conId);

                    //ibClient.SecurityDefinitionOptionParameter += OnSecurityDefinitionOptionParameter;
                    //ibClient.SecurityDefinitionOptionParameterEnd += OnSecurityDefinitionOptionParameterEnd;

                    // TODO
                    result.Add($"{ticker} {conId}");

                });
            }

            return result;
        }
    }
}
