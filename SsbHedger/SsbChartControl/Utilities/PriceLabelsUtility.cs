using SsbHedger.SsbChartControl.WpfConverters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SsbHedger.SsbChartControl.Utilities
{
    public class PriceLabelsUtility : IPriceLabelsUtility
    {
        private IRoundingUtility _roundingUntility;

        public PriceLabelsUtility()
        {
            _roundingUntility = new RoundingUtility();
        }

        public List<double> GetPrices(
            int numberOfLabels,
            double rangeMin,
            double rangeMax)
        {
            var resultList = new List<double>();

            int maxDecimalPlaces = GetMaxDecimalPlaces(rangeMin, rangeMax);
            double labelStep = (rangeMax - rangeMin) / (numberOfLabels-1);
            labelStep = Math.Round(
                labelStep,
                maxDecimalPlaces,
                MidpointRounding.AwayFromZero);

            resultList.Add(rangeMax);
            double nextPrice;
            while ((nextPrice = resultList[resultList.Count - 1] - labelStep) > rangeMin)
            {
                resultList.Add(Math.Round(nextPrice, maxDecimalPlaces));
            }
            resultList.Add(rangeMin);

            resultList = RoundUsingTwoLastDigitsArray(
                   resultList,
                   WpfConvertersConstants.TWO_LAST_DIGITS_ARRAY_STRING,
                   rangeMax,
                   maxDecimalPlaces);

            return resultList;
        }

        public List<int> GetCanvasTops(
            double axisHeight,
            double rangeMin,
            double rangeMax,
            List<double> labelPrices)
        {
            var resultList = new List<int>();
            var buffer = WpfConvertersConstants.CHART_BUFFER_UP_DOWN_IN_PERCENT;

            int maxDecimalPlaces = GetMaxDecimalPlaces(rangeMin, rangeMax);
            //(double rangeMinNet, double rangeMaxNet) = GetRangeMinMaxNet(
            //    rangeMin,
            //    rangeMax,
            //    maxDecimalPlaces,
            //    buffer
            //    );
            double range = rangeMax - rangeMin;
            //double rangeNet = rangeMaxNet - rangeMinNet;
            int offset = (int)Math.Ceiling(axisHeight * buffer / 100);
            var axisHeightNet = axisHeight - 2 * offset;
            double priceUnitInPoints = range / axisHeightNet;
            //double offset = range * buffer / (2 * 100);
            //int offsetInPoints = (int)Math.Ceiling(offset * priceUnitInPoints);

            resultList.Add(offset);
            for (int i = 1; i < labelPrices.Count; i++)
            {
                double price = labelPrices[i];
                double firstPrice = labelPrices[0];
                int top = (int)Math.Ceiling((firstPrice - price) * priceUnitInPoints);
                resultList.Add(top);
            }

            return resultList;
        }

        public int GetNumberOfLabels(double axisHeight, double chartBuffer, double minHeightForLabel)
        {
            double axisHeightNet = axisHeight * (1 - 2 * chartBuffer / 100);
            int numberOfLabels = (int)Math.Round(axisHeightNet / minHeightForLabel, 0);
            return numberOfLabels;
        }

        public (double rangeMin, double rangeMax) GetRangeMinMax(List<BarUnderlying> bars)
        {
            BarUnderlying? barWithLowestLow = bars.MinBy(b => b.Low);
            BarUnderlying? barWithHighesHigh = bars.MaxBy(b => b.High);

            if (barWithLowestLow == null || barWithHighesHigh == null)
            {
                throw new ApplicationException("Unexpected! Range low - high can not be calculated.");
            }

            return ValueTuple.Create(barWithLowestLow.Low, barWithHighesHigh.High);
        }

        private int GetMaxDecimalPlaces(double rangeMin, double rangeMax)
        {
            // TODO implement if necessary
            return 2;
        }

        private List<double> RoundUsingTwoLastDigitsArray(
            List<double> priceList,
            string twoLastDigitsArrayString,
            double rangeMaxNet,
            int maxDecimalPlaces)
        {
            var twoLastDigitsList = twoLastDigitsArrayString
                .Split(";")
                .ToList();

            foreach (string twoLastDigits in twoLastDigitsList)
            {
                List<double> resultList =
                    priceList
                    .Select(p => _roundingUntility.RoundUsingTwoLastDigits(
                        p,
                        twoLastDigits))
                    .ToList();

                if (resultList.Max() <= rangeMaxNet
                    && resultList.SequenceEqual(resultList.Distinct())
                    && SameStep(resultList, maxDecimalPlaces))
                {
                    return resultList;
                }
            }

            return priceList;
        }

        private (double rangeMinNet, double rangeMaxNet) GetRangeMinMaxNet(
            double rangeMin,
            double rangeMax,
            int maxDecimalPlaces,
            double chartBuffer)
        {
            double range = rangeMax - rangeMin;
            double offsetAbs = range * chartBuffer / 100;
            offsetAbs = Math.Round(
                offsetAbs,
                maxDecimalPlaces,
                MidpointRounding.AwayFromZero);
            double rangeMinNet = rangeMin + offsetAbs;
            double rangeMaxNet = rangeMax - offsetAbs;

            return ValueTuple.Create(rangeMinNet, rangeMaxNet);
        }

        private bool SameStep(List<double> resultList, int maxDecimalPlaces)
        {
            var stepList = new List<int>();

            for (int i = 1; i < resultList.Count; i++)
            {
                var currentValue = resultList[i];
                var valueBefore = resultList[i - 1];
                var step = currentValue - valueBefore;
                var stepMultiplied = step * Math.Pow(10, maxDecimalPlaces);
                int stepRounded = (int)Math.Round(
                    stepMultiplied,
                    maxDecimalPlaces,
                    MidpointRounding.AwayFromZero);
                stepList.Add(stepRounded);
                if (stepList.Max() != stepList.Min())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
