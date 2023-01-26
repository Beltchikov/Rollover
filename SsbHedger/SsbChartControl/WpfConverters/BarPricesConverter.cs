using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class BarPricesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var bars = (List<BarUnderlying>)values[0];
            var axisHight = (double)values[1];

            // TODO
            return new List<string> { "100", "101", "102", "13" };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
