using System.Collections.Generic;

namespace Dsmn.DataProviders
{
    public interface IInvestingProvider
    {
        List<string> GetEarningsData(string htmlSource);
    }
}