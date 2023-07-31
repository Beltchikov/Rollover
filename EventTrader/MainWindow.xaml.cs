using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
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

namespace EventTrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WebScraper _webScraper; 
        
        public MainWindow()
        {
            InitializeComponent();

            _webScraper = new WebScraper();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double ir = _webScraper.AudInterestRate();
            MessageBox.Show(ir.ToString(new CultureInfo("DE-de")));
        }
    }
}
