using System.Windows;

namespace SignalAdvisor.Commands
{
    public class SendOrders
    {
        public static void Run(IPositionsVisitor visitor)
        {
            MessageBox.Show($"SendOrders. TODO: call PlaceOrder for {visitor.InstrumentToTrade.Symbol}");
            return;
        }
    }
}
