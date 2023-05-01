using Microsoft.VisualBasic;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
            var timeCollection= ((ObservableCollection<object>)values[1]).Cast<DateTime>().ToList();
            var width = (double)values[2];

            if(width == 0)
            {
                return 0.0;
            }

            int index = timeCollection.IndexOf(time);
            var range = (timeCollection.Max() - timeCollection.Min()).Ticks * (timeCollection.Count +1) / timeCollection.Count;
            double koef = width / range;
            var offset = (time - timeCollection[0]).Ticks;

            return index == 0 ? 0 : koef * offset;

            //
            //var result = _mathUtility.GetCoordinate(chartWidth, datapoints, idx, startOffset, endOffset);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
