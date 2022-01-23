using IBApi;
using Rollover.Ib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollover.Tracking
{
    public class OrderManager : IOrderManager
    {
        private IRepository _repository;

        public OrderManager(IRepository repository)
        {
            _repository = repository;
        }

        public void RolloverIfNextStrike(ITrackedSymbols trackedSymbols)
        {
            foreach (var trackedSymbol in trackedSymbols)
            {
                var priceUnderlying = _repository.GetCurrentPrice(trackedSymbol.ConId, trackedSymbol.Exchange);
                if (!priceUnderlying.Item1)
                {
                    throw new ApplicationException($"Was not able to get a price for {trackedSymbol.ConId} and {trackedSymbol.Exchange}");
                }
                if (priceUnderlying.Item2 > trackedSymbol.NextStrike(_repository, priceUnderlying.Item2))
                {
                    var contract = new Contract()
                    {
                        Symbol = trackedSymbol.Symbol(_repository),
                        SecType = trackedSymbol.SecType(_repository),    
                        Currency = trackedSymbol.Currency(_repository),
                        Exchange = trackedSymbol.Exchange
                    };

                    //_repository.PlaceBearSpread(contract, trackedSymbol.SellConId, trackedSymbol.BuyConId);
                }
            }

            //throw new NotImplementedException();

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
