using SignalAdvisor.Controls;
using SignalAdvisor.Model;
using System.Windows;

namespace SignalAdvisor
{
    /// <summary>
    /// Interaction logic for BuyWindow.xaml
    /// </summary>
    public partial class AdvisorWindow : Window
    {
        public AdvisorWindow()
        {
            InitializeComponent();
        }

        private void InstrumentsControl_TradeAction(object sender, TradeActionEventArgs e)
        {
            var instrument = e.Instrument;

            MessageBox.Show($"TODO: call PlaceOrder for {instrument.Symbol}");

        }
    }
}
