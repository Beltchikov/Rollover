﻿using SignalAdvisor.Controls;
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
            var model = (DataContext as AdvisorViewModel) ?? throw new Exception();
            model.InstrumentToTrade = e.Instrument;
            model.SendOrdersCommand.Execute(model);
        }

        private void InstrumentsControl_TradeNonBracketAction(object sender, TradeActionEventArgs e)
        {
            var model = (DataContext as AdvisorViewModel) ?? throw new Exception();
            model.InstrumentToTrade = e.Instrument;
            model.SendNonBracketOrdersCommand.Execute(model);
        }
    }
}
