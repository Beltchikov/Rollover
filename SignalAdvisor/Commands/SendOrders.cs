using IBApi;
using System.Windows;

namespace SignalAdvisor.Commands
{
    public class SendOrders
    {
        public static async Task RunAsync(IPositionsVisitor visitor)
        {

            MessageBox.Show($"TODO SendOrders");

            //Contract contract = new Contract()
            //{
            //    ConId = visitor.InstrumentToTrade.ConId,
            //    Symbol = visitor.InstrumentToTrade.Symbol,
            //    SecType = App.SEC_TYPE_STK,
            //    Currency = visitor.InstrumentToTrade.Currency,
            //    Exchange = visitor.InstrumentToTrade.Exchange
            //};

            //double askPrice = 0;
            //var askPriceTickType = 2;
            //var timeout = App.TIMEOUT;
            ////var timeout = 60000;
            //var startTime = DateTime.Now;

            //var requestId = visitor.IbHost.RequestMktData(
            //    contract,
            //    "",
            //    false,
            //    false,
            //    null,
            //    p =>
            //    {
            //        if (p.Field == askPriceTickType)
            //            askPrice = p.Price;
            //    },
            //    s => { },
            //    (r, c, m1, m2, ex) => { });

            //await Task.Run(() =>
            //{
            //    while (askPrice == 0 && (DateTime.Now - startTime).TotalMilliseconds < timeout) { };
            //    //while (askPrice == 0) { };
            //    var todo = 0;
            //});

            ////visitor.IbHost.CancelMktData(requestId);
            //if (askPrice == 0) MessageBox.Show($"Ask price can not be obtained in {timeout} milliseconds.");
            //else MessageBox.Show($"Ask price {askPrice}");
        }
    }
}
