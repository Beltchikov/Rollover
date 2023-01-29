using System.Collections.Generic;

namespace SsbHedger.SsbChartControl.Utilities
{
    public interface IPriceLabelsUtility
    {
        List<int> GetCanvasTops(
            double axisHeightNet,
            double rangeMin,
            double rangeMax,
            List<double> labelPrices);
        List<double> GetPrices(
            int numberOfLabels,
            double rangeMin,
            double rangeMax);
    }
}