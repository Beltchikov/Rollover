using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChartControls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Chart : UserControl
    {
        //public List<object> XValues { get; set; }
        

        public string Rrr
        {
            get { return (string)GetValue(RrrProperty); }
            set { SetValue(RrrProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rrr.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RrrProperty =
            DependencyProperty.Register("Rrr", typeof(string), typeof(Chart), new PropertyMetadata(""));



        public List<object> XValues
        {
            get { return (List<object>)GetValue(XValuesProperty); }
            set { SetValue(XValuesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XValues.  This enables animation, styling, binding, etc...
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

            Rrr = "aaa";
        }
    }
}