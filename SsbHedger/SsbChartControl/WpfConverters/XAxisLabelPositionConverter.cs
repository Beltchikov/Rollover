using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class XAxisLabelPositionConverter : GridRectConverter, IMultiValueConverter
    {
        public new object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[1].GetType() != typeof(Dictionary<DateTime, bool>))
            {
                return new Rect(
                    0,
                    0,
                    WpfConvertersConstants.DEFAULT_GRID_WIDTH,
                    WpfConvertersConstants.DEFAULT_GRID_HEIGHT);
            }

            int index = (int)values[0];

            Dictionary<DateTime, bool> lineTimesDictionary = (Dictionary<DateTime, bool>)values[1];
            int barWidth = (int)values[2];
            int yAxisWidth = (int)values[3];
            double controlWidth = (double)values[4];

            object[] valuesForBaseConverter =
            {
                lineTimesDictionary,
                barWidth,
                yAxisWidth,
                controlWidth
            };

            Rect rect = (Rect)base.Convert(valuesForBaseConverter, targetType, parameter, culture);
            double left = rect.Left + rect.Width * index;
            return new Thickness(left, 0, 0, 0);
            
        }

        public new object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
