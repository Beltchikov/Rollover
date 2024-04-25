using IBApi;
using PortfolioTrader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TickType = IbClient.Types.TickType;

namespace PortfolioTrader.Commands
{
    internal class CalculatePositions
    {
        private static IBuyConfirmationModelVisitor _visitor=null!;

        public static async Task RunAsync(IBuyConfirmationModelVisitor visitor)
        {
            _visitor = visitor;
            _visitor.StocksToBuyAsString = await AddPriceColumnAsync();
        }

        private static async Task<string> AddPriceColumnAsync()
        {
            //BuyConfirmationViewModel model = DataContext as BuyConfirmationViewModel
            //    ?? throw new Exception("Unexpected. model is null");

            var stocksToBuyDictionary = SymbolsAndScore.StringToPositionDictionary(_visitor.StocksToBuyAsString);
            foreach (var kvp in stocksToBuyDictionary)
            {
                if (kvp.Value.ConId == null) throw new Exception("Unexpected. Contract ID is null");
                var contract = new Contract() { ConId = kvp.Value.ConId.Value, Exchange = App.EXCHANGE };
                (double? price, TickType? tickType) = await _visitor.IbHost.RequestMktData(contract, "", true, false, null, App.TIMEOUT);

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
