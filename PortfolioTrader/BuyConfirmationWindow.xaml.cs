using PortfolioTrader.Commands;
using PortfolioTrader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PortfolioTrader
{
    /// <summary>
    /// Interaction logic for BuyConfirmationWindow.xaml
    /// </summary>
    public partial class BuyConfirmationWindow : Window
    {
        public BuyConfirmationWindow(IBuyModelVisitor visitor)
        {
            InitializeComponent();
            DataContext = new BuyConfirmationViewModel() { 
                ConnectedToTws = visitor.ConnectedToTws,
                StocksToBuyAsString = visitor.LongSymbolsAsString,
                StocksToSellAsString = visitor.ShortSymbolsAsString
            };

            ApplyBusinessRules();
        }

        private void ApplyBusinessRules()
        {
            var model = DataContext as BuyConfirmationViewModel;
            if (model == null) throw new Exception("Unexpected. model is null");

            var stocksToBuyDictionary = SymbolsAndScore.StringToDictionary(model.StocksToBuyAsString);
            var stocksToSellDictionary = SymbolsAndScore.StringToDictionary(model.StocksToSellAsString);

            if (stocksToBuyDictionary.Count > App.MAX_BUY_SELL)
            {
                stocksToBuyDictionary = stocksToBuyDictionary.Take(App.MAX_BUY_SELL).ToDictionary();
                model.StocksToBuyAsString = SymbolsAndScore.DictionaryToString(stocksToBuyDictionary);
                AddBusinessInformation($"The number of stocks in the buy list was reduced to {App.MAX_BUY_SELL} according to the business rules.");
            }
            if (stocksToSellDictionary.Count > App.MAX_BUY_SELL)
            {
                stocksToSellDictionary = stocksToSellDictionary.Take(App.MAX_BUY_SELL).ToDictionary();
                model.StocksToSellAsString = SymbolsAndScore.DictionaryToString(stocksToSellDictionary);
                AddBusinessInformation($"The number of stocks in the sell list was reduced to {App.MAX_BUY_SELL} according to the business rules.");
            }
            if (stocksToBuyDictionary.Count != stocksToSellDictionary.Count)
            {
                var moreInBuyList = stocksToBuyDictionary.Count - stocksToSellDictionary.Count;
                if (moreInBuyList > 0)
                {
                    var equalQty = stocksToBuyDictionary.Count - moreInBuyList;
                    stocksToBuyDictionary = stocksToBuyDictionary.Take(equalQty).ToDictionary();
                    model.StocksToBuyAsString = SymbolsAndScore.DictionaryToString(stocksToBuyDictionary);
                    AddBusinessInformation("The number of stocks in the buy lists was reduced to {equalQty} to equal the sell list.");
                }
                else
                {
                    var equalQty = stocksToSellDictionary.Count + moreInBuyList;
                    stocksToSellDictionary = stocksToSellDictionary.Take(equalQty).ToDictionary();
                    model.StocksToSellAsString = SymbolsAndScore.DictionaryToString(stocksToSellDictionary);
                    AddBusinessInformation("The number of stocks in the sell lists was reduced to {equalQty} to equal the buy list.");
                }

                model.StocksToSellAsString = SymbolsAndScore.DictionaryToString(stocksToSellDictionary.Take(App.MAX_BUY_SELL).ToDictionary());
                AddBusinessInformation($"The number of stocks in the sell list was reduced to {App.MAX_BUY_SELL} according to the business rules.");
            }
        }


        private void AddBusinessInformation(string information)
        {
            var model = DataContext as BuyConfirmationViewModel;
            if (model == null) throw new Exception("Unexpected. model is null");

            if (model.BusinessLogicInformation != "") model.BusinessLogicInformation += "{\r\n}";
            model.BusinessLogicInformation += information;
        }
    }
}
