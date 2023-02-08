using SsbHedger.SsbChartControl.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class XAxisLabelPositionConverter : IMultiValueConverter
    {
        IGridLinesUtility _gridLinesUtility;

        public XAxisLabelPositionConverter()
        {
            _gridLinesUtility = new GridLinesUtility();
        }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int index = (int)values[0];
            if (values[1].GetType() != typeof(Dictionary<DateTime, bool>))
            {
                return new Rect(
                    0,
                    0,
                    WpfConvertersConstants.DEFAULT_GRID_WIDTH,
                    WpfConvertersConstants.DEFAULT_GRID_HEIGHT);
            }
            Dictionary<DateTime, bool> lineTimesDictionary = (Dictionary<DateTime, bool>)values[1];
            int barWidth = (int)values[2];
            int yAxisWidth = (int)values[3];
            double controlWidth = (double)values[4];

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

            double left = scaledOffsetX + scaledWidth * index;
            return new Thickness(left, 0, 0, 0);
            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
