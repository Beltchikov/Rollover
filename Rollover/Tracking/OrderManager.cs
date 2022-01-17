using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollover.Tracking
{
    public class OrderManager : IOrderManager
    {
        public void RolloverIfNextStrike(ITrackedSymbols trackedSymbols)
        {
            // foreach
            
            // var conId = trackedSymbol.ConId
            // var conIdUnderlying = trackedSymbol.ConIdUnderlying 

            // var priceUnderlying = _repository.GetCurrentPrice(conIdUnderlying)
            //if(priceUnderlying > trackedSymbol.NextStrike)
                // _repository.PlaceBearSpread(contract, sellConId, buyConId)


            throw new NotImplementedException();

        //    var exchange = string.IsNullOrWhiteSpace(txtExchageComboOrder.Text)
        //? null
        //: txtExchageComboOrder.Text;

        //    Contract contract = new Contract
        //    {
        //        Symbol = txtSymbolComboOrder.Text,
        //        SecType = txtSecTypeComboOrder.Text,
        //        Exchange = txtExchageComboOrder.Text,
        //        Currency = txtCurrencyComboBox.Text
        //    };

        //    // Add legs
        //    var sellLeg = new ComboLeg()
        //    {
        //        Action = "SELL",
        //        ConId = Convert.ToInt32(txtSellLegConId.Text),
        //        Ratio = 1,
        //        Exchange = exchange
        //    };
        //    var buyLeg = new ComboLeg()
        //    {
        //        Action = "BUY",
        //        ConId = Convert.ToInt32(txtBuyLegConId.Text),
        //        Ratio = 1,
        //        Exchange = exchange
        //    };
        //    contract.ComboLegs = new List<ComboLeg>();
        //    contract.ComboLegs.AddRange(new List<ComboLeg> { sellLeg, buyLeg });

        //    Order order = new Order
        //    {
        //        Action = txtActionComboBox.Text,
        //        OrderType = "LMT",
        //        TotalQuantity = Convert.ToInt32(txtQuantityComboOrder.Text),
        //        LmtPrice = Double.Parse(txtLimitPriceComboOrder.Text)
        //    };

        //    ibClient.ClientSocket.placeOrder(_nextOrderId, contract, order);
        //    ibClient.ClientSocket.reqIds(-1);
        }
    }
}
