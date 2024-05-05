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
    /// Interaction logic for PairOrdersConfirmationWindow.xaml
    /// </summary>
    public partial class PairOrdersConfirmationWindow : Window
    {
        public PairOrdersConfirmationWindow(IBuyModelVisitor visitor)
        {
            InitializeComponent();
            DataContext = new PairOrdersConfirmationViewModel()
            {
                ConnectedToTws = visitor.ConnectedToTws,
                StocksToBuyAsString = visitor.LongSymbolsResolved,
                StocksToSellAsString = visitor.ShortSymbolsResolved
            };

            ApplyBusinessRules();
        }

        private void ApplyBusinessRules()
        {
            var model = DataContext as BuyConfirmationViewModel;
            if (model == null) throw new Exception("Unexpected. model is null");

            var stocksToBuyDictionary = SymbolsAndScore.StringToPositionDictionary(model.StocksToBuyAsString);
            var stocksToSellDictionary = SymbolsAndScore.StringToPositionDictionary(model.StocksToSellAsString);

            if (stocksToBuyDictionary.Count > App.MAX_BUY_SELL)
            {
                stocksToBuyDictionary = stocksToBuyDictionary.Take(App.MAX_BUY_SELL).ToDictionary();
                model.StocksToBuyAsString = SymbolsAndScore.PositionDictionaryToString(stocksToBuyDictionary);
            }
            if (stocksToSellDictionary.Count > App.MAX_BUY_SELL)
            {
                stocksToSellDictionary = stocksToSellDictionary.Take(App.MAX_BUY_SELL).ToDictionary();
                model.StocksToSellAsString = SymbolsAndScore.PositionDictionaryToString(stocksToSellDictionary);
            }
            if (stocksToBuyDictionary.Count != stocksToSellDictionary.Count)
            {
                var moreInBuyList = stocksToBuyDictionary.Count - stocksToSellDictionary.Count;
                if (moreInBuyList > 0)
                {
                    var equalQty = stocksToBuyDictionary.Count - moreInBuyList;
                    stocksToBuyDictionary = stocksToBuyDictionary.Take(equalQty).ToDictionary();
                    model.StocksToBuyAsString = SymbolsAndScore.PositionDictionaryToString(stocksToBuyDictionary);
                }
                else
                {
                    var equalQty = stocksToSellDictionary.Count + moreInBuyList;
                    stocksToSellDictionary = stocksToSellDictionary.Take(equalQty).ToDictionary();
                    model.StocksToSellAsString = SymbolsAndScore.PositionDictionaryToString(stocksToSellDictionary);
                }
            }
        }
    }
}
