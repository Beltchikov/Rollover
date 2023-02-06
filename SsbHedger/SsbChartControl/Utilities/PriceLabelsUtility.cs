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
            (double rangeMinNet, double rangeMaxNet) = GetRangeMinMaxNet(
                rangeMin,
                rangeMax,
                maxDecimalPlaces,
                WpfConvertersConstants.CHART_BUFFER_UP_DOWN_IN_PERCENT);

            double labelStep = (rangeMaxNet - rangeMinNet) / numberOfLabels;
            labelStep = Math.Round(
                labelStep,
                maxDecimalPlaces,
                MidpointRounding.AwayFromZero);
            double halfOfLabelStep = Math.Round(
                labelStep/2,
                maxDecimalPlaces,
                MidpointRounding.AwayFromZero);

            resultList.Add(rangeMaxNet - halfOfLabelStep);
            double nextPrice;
            while ((nextPrice = resultList[resultList.Count - 1] - labelStep) > rangeMinNet)
            {
               resultList.Add(Math.Round(nextPrice, maxDecimalPlaces));
            }

            resultList = RoundUsingTwoLastDigitsArray(
                   resultList,
                   WpfConvertersConstants.TWO_LAST_DIGITS_ARRAY_STRING,
                   rangeMaxNet,
                   maxDecimalPlaces);

            return resultList;
        }

        public List<int> GetCanvasTops(
            double axisHeightNet,
            double rangeMin,
            double rangeMax,
            List<double> labelPrices)
        {
            var resultList = new List<int>();
            var buffer = WpfConvertersConstants.CHART_BUFFER_UP_DOWN_IN_PERCENT;

            int maxDecimalPlaces = GetMaxDecimalPlaces(rangeMin, rangeMax);
            (double rangeMinNet, double rangeMaxNet) = GetRangeMinMaxNet(
                rangeMin,
                rangeMax,
                maxDecimalPlaces,
                buffer
                );
            double range = rangeMax - rangeMin;
            double rangeNet = rangeMaxNet - rangeMinNet;
            double priceUnitInPoints = axisHeightNet / rangeNet;
            double offset = range * buffer / (2 * 100);
            int offsetInPoints = (int)Math.Ceiling(offset * priceUnitInPoints);

            resultList.Add(offsetInPoints);
            for (int i = 1; i < labelPrices.Count; i++)
            {
                double price = labelPrices[i];
                double firstPrice = labelPrices[0];
                int top = (int)Math.Ceiling((firstPrice - price) * priceUnitInPoints);
                resultList.Add(top);
            }
            
            return resultList;
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

            foreach(string twoLastDigits in twoLastDigitsList)
            {
                List<double> resultList = 
                    priceList
                    .Select(p => _roundingUntility.RoundUsingTwoLastDigits(
                        p,
                        twoLastDigits))
                    .ToList();

                if(resultList.Max() <= rangeMaxNet 
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
                var stepMultiplied =step * Math.Pow(10, maxDecimalPlaces);
                int stepRounded = (int)Math.Round(
                    stepMultiplied,
                    maxDecimalPlaces,
                    MidpointRounding.AwayFromZero);
                stepList.Add(stepRounded);
                if(stepList.Max() != stepList.Min())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
