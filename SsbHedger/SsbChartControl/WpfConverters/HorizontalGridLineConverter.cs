using SsbHedger.SsbChartControl.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class HorizontalGridLineConverter : GridRectConverter, IMultiValueConverter
    {
        IGridLinesUtility _gridLinesUtility;

        public HorizontalGridLineConverter()
        {
            _gridLinesUtility = new GridLinesUtility();
        }

        public new object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0].GetType() != typeof(Dictionary<DateTime, bool>))
            {
                return new Rect(
                    0,
                    0,
                    WpfConvertersConstants.DEFAULT_GRID_WIDTH,
                    WpfConvertersConstants.DEFAULT_GRID_HEIGHT);
            }

            Dictionary<DateTime, bool> lineTimesDictionary = (Dictionary<DateTime, bool>)values[0];
            int barWidth = (int)values[1];
            int yAxisWidth = (int)values[2];
            double controlWidth = (double)values[3];

            double scaledWidth = _gridLinesUtility.GetScaledWidth(
                lineTimesDictionary,
                barWidth,
                yAxisWidth,
                controlWidth);
            return new Point(scaledWidth, 0);
        }

        public new object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
