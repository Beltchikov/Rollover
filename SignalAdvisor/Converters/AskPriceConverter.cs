using System.Globalization;
using System.Windows.Data;

namespace SignalAdvisor.Converters
{
    public class AskPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            var valueTyped = (double)value;
            return valueTyped.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
