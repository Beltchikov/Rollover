﻿using ChartControls.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ChartControls.Converters
{
    public class YValuesConverter : IMultiValueConverter
    {
        private IMathUtility _mathUtility;

        public YValuesConverter()
        {
            _mathUtility = new MathUtility();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0].GetType() != typeof(double))
            {
                throw new NotImplementedException($"Not implements for the type {values.GetType()}");
            }

            var yValue = (double)values[0];
            var dataRow = ((ObservableCollection<DataPoint>)values[1]).Select(c => c.YValue).Cast<double>().ToList();
            var seriesCollection = ((ObservableCollection<ObservableCollection<DataPoint>>)values[2]);
            var height = (double)values[3];

            if (height == 0)
            {
                return 0.0;
            }

            List<List<double>> valuesOfAllSeries = GenerateDataRowsCollection(seriesCollection);
            var startOffset = (valuesOfAllSeries.SelectMany(x => x).Max() - valuesOfAllSeries.SelectMany(x => x).Min()) * 0.1;
            var endOffset = startOffset;
            
            return _mathUtility.GetDiagramY(
                height,
                valuesOfAllSeries,
                dataRow,
                dataRow.IndexOf(yValue),
                startOffset, endOffset);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private List<List<double>> GenerateDataRowsCollection(ObservableCollection<ObservableCollection<DataPoint>> dataRowsCollection)
        {
            List<List<double>> resultListOfLists = new List<List<double>>();

            foreach (var dataRow in dataRowsCollection)
            {
                var resultList = dataRow.Select(c => c.YValue).Cast<double>().ToList();
                resultListOfLists.Add(resultList);
            }

            return resultListOfLists;
        }
    }
}
