using System;
using System.Collections.Generic;

namespace SsbHedger.SsbChartControl
{
    public interface ILineValuesConverter
    {
        List<DateTime> LineTiems(DateTime sessionStart, DateTime sessionEnd);
    }
}