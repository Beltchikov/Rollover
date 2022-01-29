using System;

namespace Rollover.Helper
{
    public class IbHelper
    {
        public static string NextContractYearAndMonth(int year, int month, int periodInMonthes)
        {
            var nextContractYearAndMonth = year.ToString()
                + (Math.Ceiling((double)month / 3) * 3).ToString();
            return nextContractYearAndMonth.Length == 5
                ? nextContractYearAndMonth.Insert(4, "0")
                : nextContractYearAndMonth; 
        }
    }
}
