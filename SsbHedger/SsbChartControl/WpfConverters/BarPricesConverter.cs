using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using SsbHedger.SsbChartControl.Utilities;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class BarPricesConverter : IMultiValueConverter
    {
        private IPriceLabelsUtility _priceLabelsUtility = null!;

        public BarPricesConverter()
        {
            _priceLabelsUtility = new PriceLabelsUtility(); 
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values[0] is not List<BarUnderlying>)
            {
                return new List<PriceAndMargin>();
            }
            
            var bars = (List<BarUnderlying>)values[0];
            var yAxisHeight = (double)values[1];

            int numberOfLabels = _priceLabelsUtility.GetNumberOfLabels(
                yAxisHeight,
                WpfConvertersConstants.CHART_BUFFER_UP_DOWN_IN_PERCENT,
                WpfConvertersConstants.MIN_HEIGHT_FOR_PRICE_LABEL);
            (double rangeMin, double rangeMax) = _priceLabelsUtility.GetRangeMinMax(bars);

            List<double> labelPrices = _priceLabelsUtility.GetPrices(numberOfLabels, rangeMin, rangeMax);
            List<int> canvasTopsList = _priceLabelsUtility.GetCanvasTops(
              yAxisHeight,
              rangeMin,
              rangeMax,
              labelPrices);

            var resultList = new List<PriceAndMargin>();
            for(int i=0; i < numberOfLabels; i++)
            {
                resultList.Add(new PriceAndMargin(
                    labelPrices[i].ToString(),
                    new Thickness(0, canvasTopsList[i], 0, 0)));
            }
            return resultList;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public record PriceAndMargin(string PriceAsString, Thickness Margin);
}
