using IBApi;
using System.Windows;

namespace SignalAdvisor.Commands
{
    public class SendOrders
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {
            MessageBox.Show($"SendOrders. TODO: call PlaceOrder for {visitor.InstrumentToTrade.Symbol}");

            Contract contract = new Contract();
            
            await visitor.IbHost.RequestMktData(
                contract,
                "",
                true,
                false,
                null,
                App.TIMEOUT * 12,
                p => { },
                s => { },
                (r, c, m1, m2, ex) => { });

            //(double? price, TickType? tickType, string error)
            //       = await visitor.IbHost.RequestMktData(contract, "", true, false, null, App.TIMEOUT * 12);


            return;
        }
    }
}
