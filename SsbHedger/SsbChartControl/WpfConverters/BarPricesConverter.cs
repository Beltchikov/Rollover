using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SsbHedger.SsbChartControl.WpfConverters
{
    public class BarPricesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var bars = (List<BarUnderlying>)values[0];
            var axisHeight = (double)values[1];

            // TODO
            return new List<PriceAndMargin> 
            { 
                new PriceAndMargin("103", new Thickness(0,0,0,0)),
                new PriceAndMargin("102", new Thickness(0,50,0,0)),
                new PriceAndMargin("101", new Thickness(0,100,0,0)),
                new PriceAndMargin("100", new Thickness(0,150,0,0))
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public record PriceAndMargin(string PriceAsString, Thickness Margin);
}
