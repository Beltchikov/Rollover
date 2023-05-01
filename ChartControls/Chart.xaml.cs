using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ChartControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Chart : UserControl
    {
        //public ObservableCollection<object> XValues
        //{
        //    get { return (ObservableCollection<object>)GetValue(XValuesProperty); }
        //    set { SetValue(XValuesProperty, value); }
        //}
        //public static readonly DependencyProperty XValuesProperty =
        //    DependencyProperty.Register("XValues", typeof(ObservableCollection<object>), typeof(Chart), new PropertyMetadata(new ObservableCollection<object>()));
        public ObservableCollection<ObservableCollection<object>> XValues
        {
            get { return (ObservableCollection<ObservableCollection<object>>)GetValue(XValuesProperty); }
            set { SetValue(XValuesProperty, value); }
        }
        public static readonly DependencyProperty XValuesProperty =
            DependencyProperty.Register("XValues", typeof(ObservableCollection<ObservableCollection<object>>), 
                typeof(Chart), new PropertyMetadata(new ObservableCollection<ObservableCollection<object>>()));

        public double DotSize
        {
            get { return (double)GetValue(DotSizeProperty); }
            set { SetValue(DotSizeProperty, value); }
        }
        public static readonly DependencyProperty DotSizeProperty =
            DependencyProperty.Register("DotSize", typeof(double), typeof(Chart), new PropertyMetadata(.0));



        public Chart()
        {
            InitializeComponent();
            //DataContext = this;
            
            XValues = new ObservableCollection<ObservableCollection<object>>();

            // TODO remove later
            var dataRow1 = new ObservableCollection<object>();
            dataRow1.Add(DateTime.Parse("30.04.2023 15:30", new CultureInfo("de-DE")));
            dataRow1.Add(DateTime.Parse("30.04.2023 15:35", new CultureInfo("de-DE")));
            dataRow1.Add(DateTime.Parse("30.04.2023 15:40", new CultureInfo("de-DE")));
            XValues.Add(dataRow1);  

            DotSize = 5;

        }
    }
}