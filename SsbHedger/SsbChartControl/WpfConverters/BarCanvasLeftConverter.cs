using System;
using System.Globalization;
using System.Windows.Data;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class BarCanvasLeftConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // TODO
            return 20.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
