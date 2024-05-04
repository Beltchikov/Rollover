﻿using IBApi;
using IbClient.IbHost;
using IbClient.messages;
using PortfolioTrader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortfolioTrader.Commands
{
    internal class SymbolCheck
    {
        private static IBuyModelVisitor _visitor = null!;

        public static async Task Run(IBuyModelVisitor visitor)
        {
            _visitor = visitor;

            await ResolveSymbolsAndLog(visitor, true);
            await ResolveSymbolsAndLog(visitor, false);
        }

        private static async Task ResolveSymbolsAndLog(IBuyModelVisitor visitor, bool isLong)
        {
            var symbolsAndScoreAsDictionary = isLong
                ? SymbolsAndScore.StringToDictionary(visitor.LongSymbolsAsString)
                : SymbolsAndScore.StringToDictionary(visitor.ShortSymbolsAsString);
           
            var startMessage = BuildResolveSymbolsStartMessage(isLong: isLong, symbolsAndScoreAsDictionary);
            visitor.TwsMessageCollection?.Add(startMessage);

            Dictionary<string, Position> resolved = null!;
            Dictionary<string, int> multiple = null!, unresolved = null!;
            await Task.Run(async () =>
                (resolved, multiple, unresolved)
                = await ResolveSymbols(symbolsAndScoreAsDictionary)
            );

            if (isLong)
            {
                visitor.LongSymbolsResolved = SymbolsAndScore.PositionDictionaryToString(resolved);
                visitor.LongSymbolsMultiple = SymbolsAndScore.DictionaryToString(multiple);
                visitor.LongSymbolsUnresolved = SymbolsAndScore.DictionaryToString(unresolved);
            }
            else
            {
                visitor.ShortSymbolsResolved = SymbolsAndScore.PositionDictionaryToString(resolved);
                visitor.ShortSymbolsMultiple = SymbolsAndScore.DictionaryToString(multiple);
                visitor.ShortSymbolsUnresolved = SymbolsAndScore.DictionaryToString(unresolved);
            }

            var endMessage = BuildResolveSymbolsEndMessage(isLong: isLong, resolved, multiple, unresolved);
            visitor.TwsMessageCollection?.Add(endMessage);
        }

        private static async Task<(Dictionary<string, Position>, Dictionary<string, int>, Dictionary<string, int>)>
            ResolveSymbols(Dictionary<string, int> longSymbolAndScoreAsDictionary)
        {
            Dictionary<string, Position> symbolsResolved = new Dictionary<string, Position>();
            Dictionary<string, int> symbolsMultiple = new Dictionary<string, int>();
            Dictionary<string, int> symbolsUnresolved = new Dictionary<string, int>();

            foreach (var symbol in longSymbolAndScoreAsDictionary.Keys)
            {
                SymbolSamplesMessage symbolSamplesMessage
                    = await _visitor.IbHost.RequestMatchingSymbolsAsync(symbol, _visitor.Timeout);
                if (symbolSamplesMessage == null)
                {
                    symbolsUnresolved.Add(symbol, longSymbolAndScoreAsDictionary[symbol]);
                }
                else
                {
                    var contractDescriptions = symbolSamplesMessage.ContractDescriptions;
                    if (contractDescriptions.Count() == 1)
                    {
                        symbolsResolved.Add(symbol, new Position()
                        {
                            NetBms = longSymbolAndScoreAsDictionary[symbol],
                            ConId = contractDescriptions.First().Contract.ConId
                        });
                    }
                    else if (contractDescriptions.Count() == 0)
                    {
                        symbolsUnresolved.Add(symbol, longSymbolAndScoreAsDictionary[symbol]);
                    }
                    else
                    {
                        var contractDescriptionsNarrowed = symbolSamplesMessage.ContractDescriptions
                            .Where(d => d.Contract.SecType == App.SEC_TYPE_STK
                                && d.Contract.Currency == App.USD
                                && d.Contract.Symbol.ToUpper() == symbol.ToUpper())
                            .ToList();

                        if (contractDescriptionsNarrowed.Count == 1)
                        {
                            symbolsResolved.Add(symbol, new Position()
                            {
                                NetBms = longSymbolAndScoreAsDictionary[symbol],
                                ConId = contractDescriptionsNarrowed.First().Contract.ConId
                            });
                        }
                        else symbolsMultiple.Add(symbol, longSymbolAndScoreAsDictionary[symbol]);
                    }
                }

                await Task.Run(() => Thread.Sleep((int)Math.Round(_visitor.Timeout * 1.5)));
            }

            return (symbolsResolved, symbolsMultiple, symbolsUnresolved);
        }

        private static string BuildResolveSymbolsStartMessage(
          bool isLong,
          Dictionary<string, int> symbols)
        {
            string longOrShort = isLong ? " LONG" : " SHORT";
            var message = $"{symbols.Count()} {longOrShort} symbols to resolve.";
            return message;
        }

        private static string BuildResolveSymbolsEndMessage(
           bool isLong,
           Dictionary<string, Position> resolved,
           Dictionary<string, int> multiple,
           Dictionary<string, int> unresolved)
        {
            var message = isLong ? "LONG" : "SHORT";
            message += $" resolved:{resolved.Count} multiple:{multiple.Count} unresolved:{unresolved.Count}";
            return message;
        }
    }
}
