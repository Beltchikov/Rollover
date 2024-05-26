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
            var i = e.Instrument;
        }
    }
}
