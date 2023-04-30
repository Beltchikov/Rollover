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
        public ObservableCollection<object> XValues
        {
            get { return (ObservableCollection<object>)GetValue(XValuesProperty); }
            set { SetValue(XValuesProperty, value); }
        }

        public static readonly DependencyProperty XValuesProperty =
            DependencyProperty.Register("XValues", typeof(ObservableCollection<object>), typeof(Chart), new PropertyMetadata(new ObservableCollection<object>()));

        public Chart()
        {
            InitializeComponent();
            //DataContext = this;
            
            XValues = new ObservableCollection<object>();

            // TODO remove later
            XValues.Add(DateTime.Parse("30.04.2023 15:30", new CultureInfo("de-DE")));  
            XValues.Add(DateTime.Parse("30.04.2023 15:35", new CultureInfo("de-DE")));  
            XValues.Add(DateTime.Parse("30.04.2023 15:40", new CultureInfo("de-DE")));

        }
    }
}