using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eomn.DataProviders
{
    public interface ITwsProvider
    {
        public event Action<string> Status;
        Task<List<string>> GetRoe(List<string> tickerListTws, int timeout);
    }
}