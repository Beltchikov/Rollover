using System;
using System.Globalization;
using System.Windows.Data;

namespace StockAnalyzer.Converters
{
    public class ConnectToTwsButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value
                ? "Disconnect"
                : "Connect to TWS";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
