using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dsmn.DataProviders
{
    public interface ITwsProvider
    {
        Task<List<string>> BidAskSpread(List<string> tickerListTws, int timeout);
    }
}