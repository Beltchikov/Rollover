using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dsmn.DataProviders
{
    public interface IYahooProvider
    {
        Task<List<string>> ExpectedEpsAsync(List<string> tickerList);
    }
}