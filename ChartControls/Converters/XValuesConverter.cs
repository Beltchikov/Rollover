using System;
using System.Globalization;
using System.Windows.Data;

namespace ChartControls.Converters
{
    public class XValuesConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0].GetType() != typeof(DateTime)) 
            {
                throw new NotImplementedException($"Not implements for the type {values.GetType()}");
            }

            var time = (DateTime)values[0]; 
            var width = (double)values[1];

            if(width == 0)
            {
                return 0.0;
            }
            
            // TODO
            return 20.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
