using System;
using System.Collections.Generic;

namespace SsbHedger.SsbChartControl
{
    public interface ILineValuesConverter
    {
        Dictionary<DateTime, bool> LineTimes(
            DateTime sessionStart,
            DateTime sessionEnd,
            int hoursInterval);
    }
}