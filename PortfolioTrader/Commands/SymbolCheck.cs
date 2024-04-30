using IBApi;
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

            // Long
            var longSymbolAndScoreAsDictionary = SymbolsAndScore.StringToDictionary(visitor.LongSymbolsAsString);
            visitor.TwsMessageCollection?.Add($"{longSymbolAndScoreAsDictionary.Count()} long symbols to resolve.");

            Dictionary<string, Position> longResolved = null!;
            Dictionary<string, int> longMultiple = null!, longUnresolved = null!;
            await Task.Run(async () =>
                (longResolved, longMultiple, longUnresolved)
                = await ResolveSymbols(longSymbolAndScoreAsDictionary)
            );

            visitor.LongSymbolsResolved = SymbolsAndScore.PositionDictionaryToString(longResolved);
            visitor.LongSymbolsMultiple = SymbolsAndScore.DictionaryToString(longMultiple);
            visitor.LongSymbolsUnresolved = SymbolsAndScore.DictionaryToString(longUnresolved);

            var longMessage = BuildLogMessage(isLong: true, longResolved, longMultiple, longUnresolved);
            visitor.TwsMessageCollection?.Add(longMessage);

            // Short
            var shortSymbolAndScoreAsDictionary = SymbolsAndScore.StringToDictionary(visitor.ShortSymbolsAsString);
            visitor.TwsMessageCollection?.Add($"{shortSymbolAndScoreAsDictionary.Count()} short symbols to resolve.");

            Dictionary<string, Position> shortResolved = null!;
            Dictionary<string, int> shortMultiple = null!, shortUnresolved = null!;
            await Task.Run(async () => (shortResolved, shortMultiple, shortUnresolved)
                = await ResolveSymbols(shortSymbolAndScoreAsDictionary));

            visitor.ShortSymbolsResolved = SymbolsAndScore.PositionDictionaryToString(shortResolved);
            visitor.ShortSymbolsMultiple = SymbolsAndScore.DictionaryToString(shortMultiple);
            visitor.ShortSymbolsUnresolved = SymbolsAndScore.DictionaryToString(shortUnresolved);

            var shortMessage = BuildLogMessage(isLong: false, shortResolved, shortMultiple, shortUnresolved);
            visitor.TwsMessageCollection?.Add(shortMessage);

            //
            visitor.SymbolsChecked = true;
            visitor.TwsMessageCollection?.Add("DONE! Check Symbols command executed.");
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



        private static string BuildLogMessage(
           bool isLong,
           Dictionary<string, Position> resolved,
           Dictionary<string, int> multiple,
           Dictionary<string, int> unresolved)
        {
            var longMessage = isLong ? "LONG" : "SHORT";
            longMessage += $" resolved:{resolved.Count} multiple:{multiple.Count} unresolved:{unresolved.Count}";
            return longMessage;
        }
    }
}
