using EventTrader.EconomicData;
using System.Windows;

namespace EventTrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IEconomicDataTrader _trader;

        public MainWindow(IEconomicDataTrader trader)
        {
            _trader = trader;

            InitializeComponent();
        }

        private void btTestUsdIr_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("USD ");
        }

        private void btStartSession_Click(object sender, RoutedEventArgs e)
        {
            EconomicDataTrade trade = new EconomicDataTrade();
            _trader.StartSession(trade);
        }
    }
}
