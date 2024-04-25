﻿using IBApi;
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
        private static IBuyConfirmationModelVisitor _visitor = null!;

        public static async Task RunAsync(IBuyConfirmationModelVisitor visitor)
        {
            _visitor = visitor;
            _visitor.StocksToBuyAsString = await AddPriceColumnAsync();
            _visitor.StocksToSellAsString = await AddPriceColumnAsync();
        }

        private static async Task<string> AddPriceColumnAsync()
        {
            var stocksToBuyDictionary = SymbolsAndScore.StringToPositionDictionary(_visitor.StocksToBuyAsString);
            var resultDictionary = new Dictionary<string, Position>();
            foreach (var kvp in stocksToBuyDictionary)
            {
                if (kvp.Value.ConId == null) throw new Exception("Unexpected. Contract ID is null");
                var contract = new Contract() { ConId = kvp.Value.ConId.Value, Exchange = App.EXCHANGE };
                (double? price, TickType? tickType) = await _visitor.IbHost.RequestMktData(contract, "", true, false, null, App.TIMEOUT);
                resultDictionary[kvp.Key] = kvp.Value;

                var priceNotNullable = price == null ? 0 : price.Value;
                int quantity = 0;
                if (priceNotNullable > 0 && kvp.Value.Weight.HasValue)
                {
                    quantity = (int)Math.Floor((double)(_visitor.InvestmentAmount * kvp.Value.Weight / 100) / priceNotNullable);
                }
                resultDictionary[kvp.Key].Quantity = quantity;
            }

            return SymbolsAndScore.PositionDictionaryToString(resultDictionary);
        }
    }
}
