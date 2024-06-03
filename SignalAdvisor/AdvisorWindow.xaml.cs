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

        private void InstrumentsControl_TradeAction1(object sender, TradeActionEventArgs e)
        {
            var model = (DataContext as AdvisorViewModel) ?? throw new Exception();
            model.InstrumentToTrade = e.Instrument;
            model.SendOrders2StdDevCommand.Execute(model);
        }

        private void InstrumentsControl_TradeAction2(object sender, TradeActionEventArgs e)
        {
            var model = (DataContext as AdvisorViewModel) ?? throw new Exception();
            model.InstrumentToTrade = e.Instrument;
            model.SendNonBracketOrdersCommand.Execute(model);
        }

        private void InstrumentsControl_TradeActionShort1(object sender, TradeActionEventArgs e)
        {
            var model = (DataContext as AdvisorViewModel) ?? throw new Exception();
            model.InstrumentToTrade = e.Instrument;
            model.SendOrders2StdDevShortCommand.Execute(model);
        }

        private void InstrumentsControl_TradeActionShort2(object sender, TradeActionEventArgs e)
        {
            MessageBox.Show("Not implemented yet");
            
            //var model = (DataContext as AdvisorViewModel) ?? throw new Exception();
            //model.InstrumentToTrade = e.Instrument;
            //model.SendNonBracketOrdersCommand.Execute(model);
        }
    }
}
