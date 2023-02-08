using System;
using System.Collections.Generic;

namespace SsbHedger.SsbChartControl.Utilities
{
    public interface IGridLinesUtility
    {
        double GetScaledOffsetX(
            Dictionary<DateTime, bool> lineTimesDictionary,
            int barWidth,
            int yAxisWidth,
            double controlWidth);
        double GetScaledWidth(
            Dictionary<DateTime, bool> lineTimesDictionary,
            int barWidth,
            int yAxisWidth,
            double controlWidth);
    }
}