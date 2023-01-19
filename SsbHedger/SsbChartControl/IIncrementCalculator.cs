using System;

namespace SsbHedger.SsbChartControl
{
    public interface IIncrementCalculator
    {
        int Calculate(DateTime sessionStart, DateTime sessionEnd);
    }
}