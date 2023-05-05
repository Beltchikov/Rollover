using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChartControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Chart : UserControl
    {
        public ObservableCollection<ObservableCollection<DataPoint>> DataPointsCollection
        {
            get { return (ObservableCollection<ObservableCollection<DataPoint>>)GetValue(DataPointsCollectionProperty); }
            set { SetValue(DataPointsCollectionProperty, value); }
        }
        public static readonly DependencyProperty DataPointsCollectionProperty =
            DependencyProperty.Register("DataPointsCollection", typeof(ObservableCollection<ObservableCollection<DataPoint>>), 
                typeof(Chart), new PropertyMetadata(new ObservableCollection<ObservableCollection<DataPoint>>()));

        public Chart()
        {
            InitializeComponent();
            
            // TODO remove later
            TestData();

        }

        private void TestData()
        {
            DataPointsCollection = new ObservableCollection<ObservableCollection<DataPoint>>();

            // 
            var dataRow1 = new ObservableCollection<DataPoint>();
            dataRow1.Add(new DataPoint
            {
                XValue = DateTime.Parse("30.04.2023 15:30", new CultureInfo("de-DE")),
                YValue = 300d,
                DotSize = 5,
                Fill = Brushes.Red
            });
            dataRow1.Add(new DataPoint
            {
                XValue = DateTime.Parse("30.04.2023 15:35", new CultureInfo("de-DE")),
                YValue = 320d,
                DotSize = 5,
                Fill = Brushes.Red
            });
            dataRow1.Add(new DataPoint
            {
                XValue = DateTime.Parse("30.04.2023 15:40", new CultureInfo("de-DE")),
                YValue = 310d,
                DotSize = 5,
                Fill = Brushes.Red
            });

            DataPointsCollection.Add(dataRow1);

            // 
            var dataRow2 = new ObservableCollection<DataPoint>();
            dataRow2.Add(new DataPoint
            {
                XValue = DateTime.Parse("30.04.2023 15:30", new CultureInfo("de-DE")),
                YValue = 400d,
                DotSize = 5,
                Fill = Brushes.Green
            });
            dataRow2.Add(new DataPoint
            {
                XValue = DateTime.Parse("30.04.2023 15:35", new CultureInfo("de-DE")),
                YValue = 420d,
                DotSize = 5,
                Fill = Brushes.Green
            });
            dataRow2.Add(new DataPoint
            {
                XValue = DateTime.Parse("30.04.2023 15:40", new CultureInfo("de-DE")),
                YValue = 410d,
                DotSize = 5,
                Fill = Brushes.Green
            });

            DataPointsCollection.Add(dataRow2);
        }
    }
}