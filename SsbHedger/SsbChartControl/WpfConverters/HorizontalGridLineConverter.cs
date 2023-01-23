using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class HorizontalGridLineConverter : GridRectConverter, IMultiValueConverter
    {
        public new object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Rect rect = (Rect)base.Convert(values, targetType, parameter, culture);
            return new Point(rect.Width, 0);
        }

        public new object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
