using IBApi;
using IbClient.messages;
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

        private static string ConvertScoreToWeights(string stocksAsString)
        {
            const int hundert = 100;
            var stocksDictionary = SymbolsAndScore.StringToPositionDictionary(stocksAsString);
            var scaler = (double)stocksDictionary.Values.Select(p => p.NetBms).Sum() / 1d;
            var stocksDictionaryWithWeights = new Dictionary<string, Position>();
            foreach (var kvp in stocksDictionary)
            {
                stocksDictionaryWithWeights.Add(kvp.Key, new Position()
                {
                    NetBms = kvp.Value.NetBms,
                    ConId = kvp.Value.ConId,
                    Weight = (int)Math.Round(kvp.Value.NetBms * hundert / scaler, 2)
                });
            }

            var sumOfWeights = stocksDictionaryWithWeights.Values.Select(p => p.Weight).Sum();
            if (sumOfWeights < hundert)
            {
                var correction = hundert - sumOfWeights;
                
                var groupsDictionary = stocksDictionaryWithWeights
                    .Select(kvp => new KeyValuePair<string, int>(kvp.Key, kvp.Value.Weight == null ? 0 : kvp.Value.Weight.Value))
                    .GroupBy(a => a.Value)
                    .ToDictionary(grp => grp.Key, grp => grp.Count());

                var keyOfLargestGroup = groupsDictionary.MaxBy(entry => entry.Value).Key;
                var keyToApplyCorrection = stocksDictionaryWithWeights.Where(kvp => kvp.Value.Weight == keyOfLargestGroup)
                    .First()
                    .Key;
                                
                stocksDictionaryWithWeights[keyToApplyCorrection].Weight += correction;
            }

            if (stocksDictionaryWithWeights.Values.Select(p => p.Weight).Sum() != hundert)
                throw new Exception($"Unexpected. Weights do not sum up to {hundert}");

            return SymbolsAndScore.PositionDictionaryToString(stocksDictionaryWithWeights);
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
