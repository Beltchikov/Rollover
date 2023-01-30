using SsbHedger.SsbChartControl.WpfConverters;
using System;
using System.Collections.Generic;

namespace SsbHedger.SsbChartControl.Utilities
{
    public class PriceLabelsUtility : IPriceLabelsUtility
    {
        public List<double> GetPrices(
            int numberOfLabels,
            double rangeMin,
            double rangeMax)
        {
            var resultList = new List<double>();

            double range = rangeMax - rangeMin;
            int maxDecimalPlaces = GetMaxDecimalPlaces(rangeMin, rangeMax);
            double offsetAbs = range * 
                WpfConvertersConstants.CHART_BUFFER_UP_DOWN_IN_PERCENT / 100;
            offsetAbs = Math.Round(
                offsetAbs,
                maxDecimalPlaces,
                MidpointRounding.AwayFromZero);
            double rangeMinNet = rangeMin + offsetAbs;
            double rangeMaxNet = rangeMax - offsetAbs;

            double labelStep = (rangeMaxNet - rangeMinNet) / numberOfLabels;
            labelStep = Math.Round(
                labelStep,
                maxDecimalPlaces,
                MidpointRounding.AwayFromZero);
            double halfOfLabelStep = Math.Round(
                labelStep/2,
                maxDecimalPlaces,
                MidpointRounding.AwayFromZero);

            // TODO
            resultList.Add(rangeMinNet + halfOfLabelStep);
            double nextPrice = 0;
            while ((nextPrice = resultList[resultList.Count - 1] + labelStep) < rangeMaxNet)
            {
                resultList.Add(nextPrice);
            }


            //resultList.Add(23);
            //resultList.Add(45);
            //resultList.Add(123);
            //resultList.Add(560);

            return resultList;
        }

        private int GetMaxDecimalPlaces(double rangeMin, double rangeMax)
        {
            // TODO implement if necessary
            return 2;
        }

        public List<int> GetCanvasTops(
            double axisHeightNet,
            double rangeMin,
            double rangeMax,
            List<double> labelPrices)
        {
            var resultList = new List<int>();

            // TODO
            resultList.Add(23);
            resultList.Add(45);
            resultList.Add(123);
            resultList.Add(560);

            return resultList;
        }
    }
}
