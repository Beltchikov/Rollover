using System;
using System.Globalization;

namespace Rollover.Ib
{
    public class RepositoryHelper : IRepositoryHelper
    {
        public bool IsInTradingHours(string tradingHoursString, DateTime dateTime)
        {
            var periods = tradingHoursString.Split(";");
            foreach(var period in periods)
            {
                var fromToArray = period.Split("-");
                if(fromToArray.Length < 2)
                {
                    continue;
                }

                var from = fromToArray[0];
                var to = fromToArray[1];
                var fromTime = DateTime.ParseExact(from, "yyyyMMdd:HHmm", CultureInfo.InvariantCulture);
                var toTime = DateTime.ParseExact(to, "yyyyMMdd:HHmm", CultureInfo.InvariantCulture);

                if (fromTime < dateTime && dateTime < toTime)
                {
                    return true;
                }

            }

            return false;
        }
    }
}
