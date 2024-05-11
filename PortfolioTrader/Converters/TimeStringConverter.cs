using System.Globalization;
using System.Windows.Data;

namespace SignalAdvisor.Converters
{
    public class TimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            DateTime valueTyped = (DateTime)value;
            return $" {EnsureTwoDigits(valueTyped.Hour)}:" +
                $"{EnsureTwoDigits(valueTyped.Minute)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DateTime.MinValue;
            }

            var valueTyped = (string)value;
            var splitted = valueTyped.Split(':');
            if (splitted.Length != 2) throw new Exception("Unexpected. Time string is in wrong format.");

            int hours, minutes;
            if (!int.TryParse(splitted[0], out hours)) throw new Exception("Unexpected. Hours are in wrong format.");
            if (!int.TryParse(splitted[1], out minutes)) throw new Exception("Unexpected. Minutes are in wrong format.");

            var result = DateTime.Now.Date.AddHours(hours).AddMinutes(minutes);
            return result;
        }

        private string EnsureTwoDigits(int month)
        {
            string result = month.ToString();
            return result.Length == 2 ? result : "0" + result;
        }
    }
}
