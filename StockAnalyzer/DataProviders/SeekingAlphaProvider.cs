using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockAnalyzer.DataProviders
{
    public class SeekingAlphaProvider : ISeekingAlphaProvider
    {
        public Task<IEnumerable<string>> PeersComparison(List<string> list, int timeout)
        {
            throw new NotImplementedException();
        }
    }
}