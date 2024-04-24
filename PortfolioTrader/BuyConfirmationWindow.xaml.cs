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
            DataContext = new BuyConfirmationViewModel()
            {
                ConnectedToTws = visitor.ConnectedToTws,
                StocksToBuyAsString = visitor.LongSymbolsResolved,
                StocksToSellAsString = visitor.ShortSymbolsResolved
            };

            ApplyBusinessRules();
            CalculateWeights();
        }

        private void CalculateWeights()
        {
            BuyConfirmationViewModel model = DataContext as BuyConfirmationViewModel
                ?? throw new Exception("Unexpected. model is null");
            model.StocksToBuyAsString = ConvertScoreToWeights(model.StocksToBuyAsString);
            model.StocksToSellAsString = ConvertScoreToWeights(model.StocksToSellAsString);
        }

        private string ConvertScoreToWeights(string stocksAsString)
        {
            var stocksDictionary = SymbolsAndScore.StringToDictionary(stocksAsString);
            var scaler = (double)stocksDictionary.Values.Sum() / 1d;
            var stocksDictionaryWithWeights = new Dictionary<string, double>();
            foreach (var kvp in stocksDictionary)
            {
                stocksDictionaryWithWeights.Add(kvp.Key, Math.Round(kvp.Value / scaler, 2));
            }

            var sum = stocksDictionaryWithWeights.Values.Sum();
            if (sum < 1)
            {
                var correction = 1 - sum;
                var groupsDictionary = stocksDictionaryWithWeights
                    .GroupBy(kvp => kvp.Value)
                    .ToDictionary(grp => grp.Key, grp => grp.Count());
                var keyOfLargestGroup = groupsDictionary.MaxBy(entry => entry.Value).Key;
                var keyToApplyCorrection = stocksDictionaryWithWeights.Where(kvp => kvp.Value == keyOfLargestGroup)
                    .OrderBy(kvp => kvp.Key)
                    .First()
                    .Key;
                stocksDictionaryWithWeights[keyToApplyCorrection] += correction;
            }

            return SymbolsAndScore.DictionaryToString(stocksDictionaryWithWeights);
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
            }
            if (stocksToSellDictionary.Count > App.MAX_BUY_SELL)
            {
                stocksToSellDictionary = stocksToSellDictionary.Take(App.MAX_BUY_SELL).ToDictionary();
                model.StocksToSellAsString = SymbolsAndScore.DictionaryToString(stocksToSellDictionary);
            }
            if (stocksToBuyDictionary.Count != stocksToSellDictionary.Count)
            {
                var moreInBuyList = stocksToBuyDictionary.Count - stocksToSellDictionary.Count;
                if (moreInBuyList > 0)
                {
                    var equalQty = stocksToBuyDictionary.Count - moreInBuyList;
                    stocksToBuyDictionary = stocksToBuyDictionary.Take(equalQty).ToDictionary();
                    model.StocksToBuyAsString = SymbolsAndScore.DictionaryToString(stocksToBuyDictionary);
                }
                else
                {
                    var equalQty = stocksToSellDictionary.Count + moreInBuyList;
                    stocksToSellDictionary = stocksToSellDictionary.Take(equalQty).ToDictionary();
                    model.StocksToSellAsString = SymbolsAndScore.DictionaryToString(stocksToSellDictionary);
                }
            }
        }
    }
}
