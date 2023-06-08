using System;
using System.Globalization;

namespace SsbHedger.Utilities
{
    public class LastTradeDateConverter : ILastTradeDateConverter
    {
        public string FromDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yy", CultureInfo.InvariantCulture)
                + dateTime.ToString("MM", CultureInfo.InvariantCulture)
                + dateTime.ToString("dd", CultureInfo.InvariantCulture);
        }
    }
}
