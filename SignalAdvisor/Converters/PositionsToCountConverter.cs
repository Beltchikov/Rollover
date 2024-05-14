using IbClient.messages;
using IBSampleApp.messages;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace SignalAdvisor.Converters
{
    public class PositionsToCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }

            var valueTyped = value as ObservableCollection<PositionMessage>;
            if (valueTyped == null) return 0;
            return valueTyped.Count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
