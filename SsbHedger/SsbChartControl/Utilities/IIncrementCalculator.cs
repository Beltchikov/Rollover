using System;

namespace SsbHedger.SsbChartControl.Utilities
{
    public interface IIncrementCalculator
    {
        int Calculate(DateTime sessionStart, DateTime sessionEnd);
    }
}