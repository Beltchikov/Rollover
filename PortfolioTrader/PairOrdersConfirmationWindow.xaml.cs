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

            (string stocksToBuyAsString, string stocksToSellAsString, List<string> removedSymbols)
                = SymbolsAndScore.EqualizeBuysAndSells(visitor.LongSymbolsResolved, visitor.ShortSymbolsResolved);

            DataContext = new PairOrdersConfirmationViewModel()
            {
                ConnectedToTws = visitor.ConnectedToTws,
                StocksToBuyAsString = stocksToBuyAsString,  // Remove later TODO
                StocksToSellAsString = stocksToBuyAsString, // Remove later TODO

                PairOrdersAsString = PairOrdersAsString(stocksToBuyAsString, stocksToSellAsString),
                StocksExcludedAfterEqualizing = removedSymbols.Any()
                    ? removedSymbols.Aggregate((r, n) => r + Environment.NewLine + n)
                    : ""
            };

            ApplyBusinessRules();
        }

        private string PairOrdersAsString(string stocksToBuyAsString, string stocksToSellAsString)
        {
            Dictionary<string, PairOrderPosition> pairOrderDictionary = new();
                        
            var stocksToBuyDictionary = SymbolsAndScore.StringToPositionDictionary(stocksToBuyAsString);
            var stocksToSellDictionary = SymbolsAndScore.StringToPositionDictionary(stocksToSellAsString);
            if (stocksToBuyDictionary.Keys.Count != stocksToSellDictionary.Keys.Count)
                throw new Exception("Unexpected. The number of symbols in the buy and sell lists is not equal.");
            
            var buyKeys = stocksToBuyDictionary.Keys.ToList(); 
            var sellKeys = stocksToSellDictionary.Keys.ToList(); 
            for (int i = 0; i < buyKeys.Count; i++)
            {
                var buyKey = buyKeys[i];
                var sellKey = sellKeys[i];
                var buyPosition = stocksToBuyDictionary[buyKey];
                var sellPosition = stocksToSellDictionary[sellKey];
                    
                pairOrderDictionary.Add($"{buyKey}-{sellKey}" , new PairOrderPosition()
                { 
                    BuyNetBms = buyPosition.NetBms,
                    BuyConId = buyPosition.ConId,
                    BuyPriceInCents = buyPosition.PriceInCents,
                    BuyPriceType = buyPosition.PriceType,
                    BuyWeight = buyPosition.Weight,
                    BuyQuantity = buyPosition.Quantity,
                    BuyMargin = buyPosition.Margin,
                    BuyMarketValue = CalculateMarketValue(buyPosition.PriceInCents, buyPosition.Quantity),

                    SellNetBms = sellPosition.NetBms,
                    SellConId = sellPosition.ConId,
                    SellPriceInCents = sellPosition.PriceInCents,
                    SellPriceType = sellPosition.PriceType,
                    SellWeight = sellPosition.Weight,
                    SellQuantity = sellPosition.Quantity,
                    SellMargin = sellPosition.Margin,
                    SellMarketValue = CalculateMarketValue(sellPosition.PriceInCents, sellPosition.Quantity),

                    TotalMargin = buyPosition.Margin + sellPosition.Margin,
                    Delta = (buyPosition.Margin + sellPosition.Margin)/(buyPosition.Margin - sellPosition.Margin)
                });


            }

            return SymbolsAndScore.PairOrderPositionDictionaryToString(pairOrderDictionary);
        }

        private int? CalculateMarketValue(int? priceInCents, int? quantity)
        {
            if(priceInCents == null) return null;   
            if(quantity == null) return null;

            return (int)Math.Floor((double)quantity.Value * (double)priceInCents.Value / 100d);
        }

        private void ApplyBusinessRules()
        {
            var model = DataContext as PairOrdersConfirmationViewModel;
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
