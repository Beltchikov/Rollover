using System;
using System.Globalization;

namespace StockAnalyzer.DataProviders.Types.EdgarApi
{
    public static class EdgarApiExtensions
    {
        public static TimeSpan FiledToEndDiff(this Currency currency)
        {
            return DateTime.ParseExact(currency.filed, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                            - DateTime.ParseExact(currency.end, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
    }
}
