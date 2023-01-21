using System;
using System.Collections.Generic;

namespace SsbHedger.SsbChartControl
{
    public interface ILineValuesConverter
    {
        List<DateTime> LineTimes(
            DateTime sessionStart,
            DateTime sessionEnd,
            int hoursInterval);
    }
}