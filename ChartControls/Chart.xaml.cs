using System;
using System.Collections.Generic;
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
        public List<object> XValues
        {
            get { return (List<object>)GetValue(XValuesProperty); }
            set { SetValue(XValuesProperty, value); }
        }

        public static readonly DependencyProperty XValuesProperty =
            DependencyProperty.Register("XValues", typeof(List<object>), typeof(Chart), new PropertyMetadata(new List<object>()));

        public Chart()
        {
            InitializeComponent();
            //DataContext = this;
            
            XValues = new List<object>();

            // TODO remove later
            XValues.Add(DateTime.Parse("30.04.2023 15:30", new CultureInfo("de-DE")));  
            XValues.Add(DateTime.Parse("30.04.2023 15:35", new CultureInfo("de-DE")));  
            XValues.Add(DateTime.Parse("30.04.2023 15:40", new CultureInfo("de-DE")));

        }
    }
}