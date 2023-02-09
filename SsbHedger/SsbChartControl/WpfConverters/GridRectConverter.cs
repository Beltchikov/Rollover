using SsbHedger.SsbChartControl.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class GridRectConverter : IMultiValueConverter
    {
        IGridLinesUtility _gridLinesUtility;

        public GridRectConverter()
        {
            _gridLinesUtility = new GridLinesUtility();
        }

        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if(values[0].GetType() != typeof(Dictionary<DateTime, bool>))
            {
                return new Rect(
                    0,
                    0,
                    WpfConvertersConstants.DEFAULT_GRID_WIDTH,
                    WpfConvertersConstants.DEFAULT_GRID_HEIGHT);
            }
            
            Dictionary<DateTime, bool> lineTimesDictionary = (Dictionary<DateTime, bool>)values[0];
            int barWidth = (int)values[1];
            int xAxisHeight = (int)values[2];
            int yAxisWidth = (int)values[3];
            double controlWidth = (double)values[4];
            double controlHeight= (double)values[5];
            var bars = (List<BarUnderlying>)values[6];

            double scaledOffsetX = _gridLinesUtility.GetScaledOffsetX(
                lineTimesDictionary,
                barWidth,
                yAxisWidth,
                controlWidth);
            double scaledWidth = _gridLinesUtility.GetScaledWidth(
                lineTimesDictionary,
                barWidth,
                yAxisWidth,
                controlWidth);

            return new Rect(scaledOffsetX, 0, scaledWidth, 20);
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
