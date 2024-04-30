﻿using IBApi;
using IbClient.Types;
using PortfolioTrader.Model;
using System.Windows.Controls;
using TickType = IbClient.Types.TickType;

namespace PortfolioTrader.Commands
{
    internal class CalculatePositions
    {
        private static IBuyConfirmationModelVisitor _visitor = null!;

        public static async Task RunAsync(IBuyConfirmationModelVisitor visitor)
        {
            _visitor = visitor;

            _visitor.StocksToBuyAsString = await AddPriceColumnsAsync(_visitor.StocksToBuyAsString);
            _visitor.StocksToSellAsString = await AddPriceColumnsAsync(_visitor.StocksToSellAsString);
            _visitor.TwsMessageCollection.Add("Calculated Position command: price column added.");

            (_visitor.StocksToBuyAsString, string stocksWithoutMarginLong) = await AddMarginColumnsAsync(_visitor.StocksToBuyAsString);
            (_visitor.StocksToSellAsString, string stocksWithoutMarginShort) = await AddMarginColumnsAsync(_visitor.StocksToSellAsString);
            _visitor.StocksWithoutMargin = stocksWithoutMarginLong + Environment.NewLine + stocksWithoutMarginShort;
            _visitor.TwsMessageCollection.Add("Calculated Position command: margin column added.");

            (_visitor.StocksToBuyAsString, string stocksToBuyWithoutPriceAsString) = RemoveZeroPriceLines(_visitor.StocksToBuyAsString);
            (_visitor.StocksToSellAsString, string stocksToSellWithoutPriceAsString) = RemoveZeroPriceLines(_visitor.StocksToSellAsString);
            _visitor.StocksWithoutPrice = stocksToBuyWithoutPriceAsString == ""
                ? stocksToSellWithoutPriceAsString
                : stocksToBuyWithoutPriceAsString + Environment.NewLine + stocksToSellWithoutPriceAsString;
            _visitor.TwsMessageCollection.Add("Calculated Position command: zero price positions removed.");

            (_visitor.StocksToBuyAsString, _visitor.StocksToSellAsString)
                = EqualizeBuysAndSells(_visitor.StocksToBuyAsString, _visitor.StocksToSellAsString);
            _visitor.TwsMessageCollection.Add("Calculated Position command: buy and sell positions equalized.");

            _visitor.CalculateWeights();
            _visitor.PositionsCalculated = _visitor.StocksToBuyAsString != "" && _visitor.StocksToSellAsString != "";
            _visitor.TwsMessageCollection.Add("Calculated Position command: weights are reccalculated after the equalizing.");

            _visitor.TwsMessageCollection.Add("DONE! Calculated Position command executed.");
        }

        private static (string, string) EqualizeBuysAndSells(string stocksToBuyAsString, string stocksToSellAsString)
        {
            var buyDictionary = SymbolsAndScore.StringToPositionDictionary(stocksToBuyAsString);
            var sellDictionary = SymbolsAndScore.StringToPositionDictionary(stocksToSellAsString);


            var min = Math.Min(buyDictionary.Count, sellDictionary.Count);
            if (buyDictionary.Count > min)
            {
                buyDictionary = RemoveDictionaryEntriesAtEnd(buyDictionary, buyDictionary.Count - min);
            }
            if (sellDictionary.Count > min)
            {
                sellDictionary = RemoveDictionaryEntriesAtEnd(sellDictionary, sellDictionary.Count - min);
            }

            return (
                SymbolsAndScore.PositionDictionaryToString(buyDictionary),
                SymbolsAndScore.PositionDictionaryToString(sellDictionary));

        }

        private static Dictionary<string, Position> RemoveDictionaryEntriesAtEnd(Dictionary<string, Position> dictionary, int removeCount)
        {
            var keysToRemove = dictionary.Keys.Reverse().Take(removeCount);
            foreach (var keyToRemove in keysToRemove)
            {
                dictionary.Remove(keyToRemove);
            }
            return dictionary;
        }

        private static (string, string) RemoveZeroPriceLines(string stocksAsString)
        {
            var stocksDictionary = SymbolsAndScore.StringToPositionDictionary(stocksAsString);
            var resultDictionary = new Dictionary<string, Position>();
            var stocksWithoutPrice = new List<string>();

            foreach (var kvp in stocksDictionary)
            {
                if (kvp.Value.PriceInCents == null || kvp.Value.PriceInCents == 0)
                {
                    stocksWithoutPrice.Add(kvp.Key);
                }
                else
                {
                    resultDictionary[kvp.Key] = kvp.Value;
                }
            }

            string stocksWithoutPriceString = SymbolsAndScore.ListToCsvString(stocksWithoutPrice, Environment.NewLine);
            //_visitor.TwsMessageCollection.Add(
            //    $"Can not retrieve price for the following symbols: {newLineSeparatedString}");

            return (SymbolsAndScore.PositionDictionaryToString(resultDictionary), stocksWithoutPriceString);
        }

        private static async Task<string> AddPriceColumnsAsync(string stocksAsString)
        {
            var stocksDictionary = SymbolsAndScore.StringToPositionDictionary(stocksAsString);
            var resultDictionary = new Dictionary<string, Position>();

            foreach (var kvp in stocksDictionary)
            {
                if (kvp.Value.ConId == null) throw new Exception("Unexpected. Contract ID is null");
                var contract = new Contract() { ConId = kvp.Value.ConId.Value, Exchange = App.EXCHANGE };

                (double? price, TickType? tickType) = await _visitor.IbHost.RequestMktData(contract, "", true, false, null, App.TIMEOUT * 11);
                resultDictionary[kvp.Key] = kvp.Value;

                int priceInCentsNotNullable = (int)((price == null ? 0d : price.Value) * 100d);
                int weightNotNullable = kvp.Value.Weight ?? throw new Exception("Unexpected. Weight is null");

                int quantity = 0;
                if (priceInCentsNotNullable > 0 && kvp.Value.Weight.HasValue)
                {
                    quantity = CalculateQuantity(_visitor.InvestmentAmount, priceInCentsNotNullable, weightNotNullable);
                }
                resultDictionary[kvp.Key].PriceInCents = priceInCentsNotNullable;
                resultDictionary[kvp.Key].PriceType = tickType.ToString();
                resultDictionary[kvp.Key].Quantity = quantity;

                await Task.Run(() => Thread.Sleep(App.TIMEOUT));
            }

            return SymbolsAndScore.PositionDictionaryToString(resultDictionary);
        }

        private static async Task<(string, string)> AddMarginColumnsAsync(string stocksAsString)
        {
            var stocksDictionary = SymbolsAndScore.StringToPositionDictionary(stocksAsString);
            var resultDictionary = new Dictionary<string, Position>();
            var positionsWithoutMargin = new List<string>();

            foreach (var kvp in stocksDictionary)
            {
                if (kvp.Value.ConId == null) throw new Exception("Unexpected. Contract ID is null");
                var contract = new Contract() { ConId = kvp.Value.ConId.Value, Exchange = App.EXCHANGE };

                if (kvp.Value.Quantity == null) throw new Exception("Unexpected. Quantity is null");
                OrderStateOrError orderStateOrError = await _visitor.IbHost.WhatIfOrderStateFromContract(contract, kvp.Value.Quantity.Value, App.TIMEOUT);

                // consider int
                if (orderStateOrError.ErrorMessage != "")
                {
                    positionsWithoutMargin.Add(kvp.Key);
                    _visitor.TwsMessageCollection.Add(orderStateOrError.ErrorMessage);

                }
                else if (orderStateOrError.OrderState != null)
                {
                    resultDictionary[kvp.Key] = kvp.Value;
                    resultDictionary[kvp.Key].Margin = orderStateOrError.OrderState.InitMarginChange;
                }
                else throw new Exception("Unexpected. Both ErrorMessage and OrderState are invalid.");

                await Task.Run(() => Thread.Sleep(App.TIMEOUT));
            }

            string positionsWithoutMarginString = SymbolsAndScore.ListToCsvString(positionsWithoutMargin, Environment.NewLine);
            return (SymbolsAndScore.PositionDictionaryToString(resultDictionary), positionsWithoutMarginString);
        }

        private static int CalculateQuantity(int investmentAmount, int priceInCents, int weight)
        {
            var priceInDollars = (double)priceInCents / 100d;
            return (int)Math.Floor((double)(investmentAmount * weight / 100) / priceInDollars);
        }
    }
}
