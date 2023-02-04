﻿using SsbHedger.SsbChartControl.WpfConverters;
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
               resultList.Add(nextPrice);
            }

            resultList = RoundUsingTwoLastDigitsArray(
                   resultList,
                   WpfConvertersConstants.TWO_LAST_DIGITS_ARRAY_STRING,
                   rangeMaxNet);

            return resultList;
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

        private int GetMaxDecimalPlaces(double rangeMin, double rangeMax)
        {
            // TODO implement if necessary
            return 2;
        }

        private List<double> RoundUsingTwoLastDigitsArray(
            List<double> priceList,
            string twoLastDigitsArrayString,
            double rangeMaxNet)
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
                    && resultList.SequenceEqual(resultList.Distinct()))
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
    }
}
