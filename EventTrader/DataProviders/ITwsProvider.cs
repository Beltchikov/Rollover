using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eomn.DataProviders
{
    public interface ITwsProvider
    {
        Task<List<string>> GetRoe(List<string> tickerListTws, int timeout);
    }
}