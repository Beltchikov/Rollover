﻿namespace Dsmn.EconomicData.Strategies
{
    /// <summary>
    /// Returns actual data on the 4-th request
    /// </summary>
    public class FakedDataProvider : IEconomicDataProvider
    {
        int callCount = 0;
        
        public (double?, double?, double?) GetData(
            string url,
            string xPathActual,
            string xPathExpected,
            string xPathPrevious,
            string nullPlaceholder)
        {
            callCount++;
            return (callCount < 4 ? null : 10.5, 10.6, 10.2);
        }
    }
}
