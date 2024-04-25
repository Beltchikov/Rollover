using PortfolioTrader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PortfolioTrader.Commands
{
    internal class CalculatePositions
    {
        private static IBuyConfirmationModelVisitor _visitor=null!;

        public static async Task RunAsync(IBuyConfirmationModelVisitor visitor)
        {
            MessageBox.Show("CalculatePositions");

            _visitor = visitor;
            _visitor.StocksToBuyAsString = await AddPriceColumnAsync();
        }

        private static async Task<string> AddPriceColumnAsync()
        {
            //BuyConfirmationViewModel model = DataContext as BuyConfirmationViewModel
            //    ?? throw new Exception("Unexpected. model is null");

            var stocksToBuyDictionary = SymbolsAndScore.StringToDictionary(_visitor.StocksToBuyAsString);
            foreach (var kvp in stocksToBuyDictionary)
            {
                // TODO




                //var contract = new Contract();
                //(double? price, TickType? tickType) = await model.IbHost.RequestMarketData(contract, "", true, false, null, App.TIMEOUT);
            }

            //private void _ibClient_TickPrice(TickPriceMessage tickPriceMessage)
            //{
            //    if (tickPriceMessage.Field == 1)  // bid. Use 2 for ask
            //    {
            //        if (tickPriceMessage.RequestId == REQ_MKT_DATA_UNDERLYING)
            //        {
            //            ViewModel.UnderlyingPrice = tickPriceMessage.Price;
            //            //_atmStrikeUtility.SetAtmStrikesInViewModel(this, tickPriceMessage.Price);
            //        }
            //    }
            //}

            return "TODO";

        }
    }
}
