using System;
using System.Collections.Generic;

namespace SsbHedger.SsbChart
{
    public interface ILineValuesConverter
    {
        List<DateTime> LineTiems(DateTime sessionStart, DateTime sessionEnd);
    }
}