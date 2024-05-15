using System.Globalization;
using System.Windows.Data;

namespace PortfolioTrader.Converters
{
    public class TimeStringConverter : IValueConverter
    {
        private readonly string formatString = "dd.MM.yyyy HH:mm";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            DateTime valueTyped = (DateTime)value;
            return valueTyped.ToString(formatString);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DateTime.MinValue;
            }

            var valueTyped = (string)value;
            if (!DateTime.TryParseExact(
                valueTyped,
                formatString,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime dateTime))
                throw new Exception("Unexpected. Hours are in wrong format.");
            return dateTime;
        }

        private string EnsureTwoDigits(int month)
        {
            string result = month.ToString();
            return result.Length == 2 ? result : "0" + result;
        }
    }
}
