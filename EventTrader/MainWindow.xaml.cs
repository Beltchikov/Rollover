using System.Globalization;
using System.Windows;

namespace EventTrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IWebScraper _webScraper;

        public MainWindow(IWebScraper webScraper)
        {
            _webScraper = webScraper;
            InitializeComponent();
        }

        private void btTestAudIr_Click(object sender, RoutedEventArgs e)
        {
            double ir = _webScraper.AudInterestRate();
            MessageBox.Show(ir.ToString(new CultureInfo("DE-de")));
        }
    }
}
