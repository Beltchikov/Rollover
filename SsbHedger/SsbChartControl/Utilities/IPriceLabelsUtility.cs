﻿using System.Collections.Generic;

namespace SsbHedger.SsbChartControl.Utilities
{
    public interface IPriceLabelsUtility
    {
        List<double> GetPrices(
            int numberOfLabels,
            double rangeMin,
            double rangeMax);

        List<int> GetCanvasTops(
            double axisHeightNet,
            double rangeMin,
            double rangeMax,
            List<double> labelPrices);
        int GetNumberOfLabels(double axisHeight, double chartBuffer, double minHeightForLabel);
        (double rangeMin, double rangeMax) GetRangeMinMax(List<BarUnderlying> bars);
    }
}