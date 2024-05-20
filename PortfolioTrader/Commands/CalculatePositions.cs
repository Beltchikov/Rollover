using IBApi;
using IbClient.Types;
using PortfolioTrader.Model;
using System.Windows;
using TickType = IbClient.Types.TickType;

namespace PortfolioTrader.Commands
{
    internal class CalculatePositions
    {
        public static async Task RunAsync(IBuyConfirmationModelVisitor visitor)
        {
            visitor.StocksToBuyAsString = await AddPriceColumnsAsync(isLong: true, visitor);
            visitor.StocksToSellAsString = await AddPriceColumnsAsync(isLong: false, visitor);
            visitor.TwsMessageCollection.Add("Calculate Positions command: price column added.");

            (visitor.StocksToBuyAsString, string stocksToBuyWithoutPriceAsString) = RemoveZeroPriceLines(visitor.StocksToBuyAsString);
            (visitor.StocksToSellAsString, string stocksToSellWithoutPriceAsString) = RemoveZeroPriceLines(visitor.StocksToSellAsString);
            visitor.StocksWithoutPrice = SymbolsAndScore.ConcatStringsWithNewLine(stocksToBuyWithoutPriceAsString, stocksToSellWithoutPriceAsString);
            visitor.TwsMessageCollection.Add("Calculate Positions command: zero price positions removed.");

            (visitor.StocksToBuyAsString, visitor.StocksToSellAsString, _)
                = SymbolsAndScore.EqualizeBuysAndSells(visitor.StocksToBuyAsString, visitor.StocksToSellAsString);
            visitor.TwsMessageCollection.Add("Calculate Positions command: buy and sell positions equalized after zero price removal.");

            visitor.CalculateWeights();
            visitor.TwsMessageCollection.Add("Calculate Positions command: weights are calculated after zero price removal.");

            visitor.StocksToBuyAsString = Position.CalculateQuantity(visitor.StocksToBuyAsString, visitor.InvestmentAmount);
            visitor.StocksToSellAsString = Position.CalculateQuantity(visitor.StocksToSellAsString, visitor.InvestmentAmount);
            visitor.TwsMessageCollection.Add("Calculate Positions command: position sizes calculted.");

            (visitor.StocksToBuyAsString, string stocksWithoutMarginLong) = await AddMarginColumnsAsync(isLong: true, visitor);
            (visitor.StocksToSellAsString, string stocksWithoutMarginShort) = await AddMarginColumnsAsync(isLong: false, visitor);
            visitor.StocksWithoutMargin = SymbolsAndScore.ConcatStringsWithNewLine(stocksWithoutMarginLong, stocksWithoutMarginShort);
            visitor.TwsMessageCollection.Add("Calculate Positions command: margin column added.");

            // Adding a margin column removes lines without margin after the addition. That's why recalculations are necessary.
            (visitor.StocksToBuyAsString, visitor.StocksToSellAsString, _)
               = SymbolsAndScore.EqualizeBuysAndSells(visitor.StocksToBuyAsString, visitor.StocksToSellAsString);
            visitor.TwsMessageCollection.Add("Calculate Positions command: buy and sell positions equalized after adding the margin column.");

            visitor.CalculateWeights();
            visitor.TwsMessageCollection.Add("Calculate Positions command: weights are recalculated after adding the margin column.");

            visitor.StocksToBuyAsString = Position.CalculateQuantity(visitor.StocksToBuyAsString, visitor.InvestmentAmount);
            visitor.StocksToSellAsString = Position.CalculateQuantity(visitor.StocksToSellAsString, visitor.InvestmentAmount);
            visitor.TwsMessageCollection.Add("Calculate Positions command: position sizes recalculted after adding the margin column.");

            visitor.ClearQueueOrderOpenMessage();
            visitor.TwsMessageCollection.Add("Calculate Positions command: open order queue cleared.");

            visitor.PositionsCalculated = visitor.StocksToBuyAsString != "" && visitor.StocksToSellAsString != "";

            var msg = $"DONE! Calculated Position command executed.";
            visitor.TwsMessageCollection.Add(msg);
            MessageBox.Show(msg);
        }

        private static (string, string) RemoveZeroPriceLines(string stocksAsString)
        {
            var stocksDictionary = SymbolsAndScore.StringToPositionDictionary(stocksAsString);
            var resultDictionary = new Dictionary<string, Position>();
            var stocksWithoutPrice = new List<string>();

            foreach (var kvp in stocksDictionary)
            {
                if (kvp.Value.PriceInCents == null || kvp.Value.PriceInCents <= 0)
                {
                    stocksWithoutPrice.Add(kvp.Key);
                }
                else
                {
                    resultDictionary[kvp.Key] = kvp.Value;
                }
            }

            string stocksWithoutPriceString = SymbolsAndScore.ListToCsvString(stocksWithoutPrice, Environment.NewLine);
            return (SymbolsAndScore.PositionDictionaryToString(resultDictionary), stocksWithoutPriceString);
        }

        private static async Task<string> AddPriceColumnsAsync(bool isLong, IBuyConfirmationModelVisitor visitor)
        {
            var stocksAsString = isLong ? visitor.StocksToBuyAsString : visitor.StocksToSellAsString;
            var stocksDictionary = SymbolsAndScore.StringToPositionDictionary(stocksAsString);
            var resultDictionary = new Dictionary<string, Position>();

            foreach (var kvp in stocksDictionary)
            {
                if (kvp.Value.ConId == null) throw new Exception("Unexpected. Contract ID is null");
                var contract = new Contract() { ConId = kvp.Value.ConId.Value, Exchange = App.EXCHANGE };

                (double? price, TickType? tickType, string error)
                    = await visitor.IbHost.RequestMktData(contract, "", true, false, null, App.TIMEOUT * 12);
                if (error != "") visitor.TwsMessageCollection.Add(error);
                resultDictionary[kvp.Key] = kvp.Value;

                int priceInCentsNotNullable = (int)((price == null ? 0d : price.Value) * 100d);
                resultDictionary[kvp.Key].PriceInCents = priceInCentsNotNullable;
                resultDictionary[kvp.Key].PriceType = tickType.ToString();

                await Task.Run(() => Thread.Sleep(App.TIMEOUT));
            }

            return SymbolsAndScore.PositionDictionaryToString(resultDictionary);
        }

        private static async Task<(string, string)> AddMarginColumnsAsync(bool isLong, IBuyConfirmationModelVisitor visitor)
        {
            var stocksAsString = isLong ? visitor.StocksToBuyAsString : visitor.StocksToSellAsString;
            var stocksDictionary = SymbolsAndScore.StringToPositionDictionary(stocksAsString);
            var resultDictionary = new Dictionary<string, Position>();
            var positionsWithoutMargin = new List<string>();

            foreach (var kvp in stocksDictionary)
            {
                if (kvp.Value.ConId == null) throw new Exception("Unexpected. Contract ID is null");
                var contract = new Contract() { ConId = kvp.Value.ConId.Value, Exchange = App.EXCHANGE };

                if (kvp.Value.Quantity == null) throw new Exception("Unexpected. Quantity is null");
                OrderStateOrError orderStateOrError = await visitor.IbHost.WhatIfOrderStateFromContract(contract, kvp.Value.Quantity.Value, App.TIMEOUT * 2);

                if (orderStateOrError.ErrorMessage != "")
                {
                    positionsWithoutMargin.Add(kvp.Key);
                    visitor.TwsMessageCollection.Add(orderStateOrError.ErrorMessage);

                }
                else if (orderStateOrError.OrderState != null)
                {
                    resultDictionary[kvp.Key] = kvp.Value;
                    resultDictionary[kvp.Key].Margin = ConvertMarginToInt(orderStateOrError.OrderState.InitMarginChange);
                }
                else throw new Exception("Unexpected. Both ErrorMessage and OrderState are invalid.");

                await Task.Run(() => Thread.Sleep(App.TIMEOUT));
            }

            string positionsWithoutMarginString = SymbolsAndScore.ListToCsvString(positionsWithoutMargin, Environment.NewLine);
            return (SymbolsAndScore.PositionDictionaryToString(resultDictionary), positionsWithoutMarginString);
        }

        private static int ConvertMarginToInt(string marginAsString)
        {
            string marginStringWithoutCents;
            if (marginAsString.Contains("."))
            {
                marginStringWithoutCents = marginAsString.Split(".")[0];
            }
            else marginStringWithoutCents = marginAsString;

            if (Int32.TryParse(marginStringWithoutCents, out int margin))
                return margin;
            else
                return 0;
        }
    }
}
