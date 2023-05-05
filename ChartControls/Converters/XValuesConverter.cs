using ChartControls.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace ChartControls.Converters
{
    public class XValuesConverter : IMultiValueConverter
    {
        private IMathUtility _mathUtility;

        public XValuesConverter()
        {
            _mathUtility = new MathUtility();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0].GetType() != typeof(DateTime)) 
            {
                throw new NotImplementedException($"Not implements for the type {values.GetType()}");
            }

            var time = (DateTime)values[0]; 
            var timeCollection= ((ObservableCollection<DataPoint>)values[1]).Select(c => c.XValue).Cast<DateTime>().ToList();
            var dataRowsCollection = ((ObservableCollection<ObservableCollection<DataPoint>>)values[2]);
            var width = (double)values[3];

            if(width == 0)
            {
                return 0.0;
            }

            List<List<double>> valuesOfAllDataRows = GenerateDataRowsCollection(dataRowsCollection);
            var valuesForDiagram = timeCollection.Select(t => (t - new DateTime(1970, 1, 1)).TotalMilliseconds)
                                                                          .Cast<double>()
                                                                          .ToList();
            var endOffset = (valuesForDiagram.Max() - valuesForDiagram.Min()) * 0.2;
            var startOffset = (valuesForDiagram.Max() - valuesForDiagram.Min()) * 0.02;
            return _mathUtility.GetDiagramX(width, valuesOfAllDataRows, valuesForDiagram, timeCollection.IndexOf(time), startOffset, endOffset);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private List<List<double>> GenerateDataRowsCollection(ObservableCollection<ObservableCollection<DataPoint>> dataRowsCollection)
        {
            List<List<double>> resultListOfLists = new List<List<double>> ();

            foreach(var dataRow in dataRowsCollection) 
            {
                var dateTimeList = dataRow.Select(c => c.XValue).Cast<DateTime>().ToList();
                var resultList = dateTimeList.Select(t => (t - new DateTime(1970, 1, 1)).TotalMilliseconds)
                                                                          .Cast<double>()
                                                                          .ToList();
                resultListOfLists.Add(resultList);
            }

            return resultListOfLists;
        }
    }
}
