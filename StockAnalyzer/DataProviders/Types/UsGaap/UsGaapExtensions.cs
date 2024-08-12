using System;
using System.Globalization;

namespace StockAnalyzer.DataProviders.Types.UsGaap
{
    public static class UsGaapExtensions
    {
        public static TimeSpan FiledToEndDiff(this USD usd)
        {
            return DateTime.ParseExact(usd.filed, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                            - DateTime.ParseExact(usd.end, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
