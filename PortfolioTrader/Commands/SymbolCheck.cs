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
        private static readonly string SEC_TYPE_STK = "STK";
        private static readonly string USD = "USD";

        public static async Task Run(IBuyModelVisitor visitor)
        {
            _visitor = visitor;

            // Long
            var longSymbolAndScoreAsDictionary = SymbolsAndScore.StringToDictionary(visitor.LongSymbolsAsString);
            visitor.TwsMessageCollection?.Add($"{longSymbolAndScoreAsDictionary.Count()} long symbols to resolve.");

            Dictionary<string, int> longResolved = null!, longMultiple = null!, longUnresolved = null!;
            await Task.Run(async () =>
                (longResolved, longMultiple, longUnresolved)
                = await ResolveSymbols(longSymbolAndScoreAsDictionary)
            );

            visitor.LongSymbolsResolved = SymbolsAndScore.DictionaryToString(longResolved);
            visitor.LongSymbolsMultiple = SymbolsAndScore.DictionaryToString(longMultiple);
            visitor.LongSymbolsUnresolved = SymbolsAndScore.DictionaryToString(longUnresolved);

            var longMessage = BuildLogMessage(isLong: true, longResolved, longMultiple, longUnresolved);
            visitor.TwsMessageCollection?.Add(longMessage);

            // Short
            var shortSymbolAndScoreAsDictionary = SymbolsAndScore.StringToDictionary(visitor.ShortSymbolsAsString);
            visitor.TwsMessageCollection?.Add($"{shortSymbolAndScoreAsDictionary.Count()} short symbols to resolve.");

            Dictionary<string, int> shortResolved = null!, shortMultiple = null!, shortUnresolved = null!;
            await Task.Run(async () => (shortResolved, shortMultiple, shortUnresolved)
                = await ResolveSymbols(longSymbolAndScoreAsDictionary));

            visitor.ShortSymbolsResolved = SymbolsAndScore.DictionaryToString(shortResolved);
            visitor.ShortSymbolsMultiple = SymbolsAndScore.DictionaryToString(shortMultiple);
            visitor.ShortSymbolsUnresolved = SymbolsAndScore.DictionaryToString(shortUnresolved);

            var shortMessage = BuildLogMessage(isLong: false, shortResolved, shortMultiple, shortUnresolved);
            visitor.TwsMessageCollection?.Add(shortMessage);
        }

        private static async Task<(Dictionary<string, int>, Dictionary<string, int>, Dictionary<string, int>)>
            ResolveSymbols(Dictionary<string, int> longSymbolAndScoreAsDictionary)
        {
            Dictionary<string, int> symbolsResolved = new Dictionary<string, int>();
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
                    if (symbolSamplesMessage.ContractDescriptions.Count() == 1)
                    {
                        symbolsResolved.Add(symbol, longSymbolAndScoreAsDictionary[symbol]);
                    }
                    else if (symbolSamplesMessage.ContractDescriptions.Count() == 0)
                    {
                        symbolsUnresolved.Add(symbol, longSymbolAndScoreAsDictionary[symbol]);
                    }
                    else
                    {
                        var stkAndUsdList = symbolSamplesMessage.ContractDescriptions
                            .Where(d => d.Contract.SecType == SEC_TYPE_STK
                                && d.Contract.Currency == USD
                                && d.Contract.Symbol.ToUpper() == symbol.ToUpper())
                            .ToList();

                        if (stkAndUsdList.Count == 1) symbolsResolved.Add(symbol, longSymbolAndScoreAsDictionary[symbol]);
                        else symbolsMultiple.Add(symbol, longSymbolAndScoreAsDictionary[symbol]);
                    }
                }

                Thread.Sleep((int)Math.Round(_visitor.Timeout * 1.5));
            }

            return (symbolsResolved, symbolsMultiple, symbolsUnresolved);
        }

        

        private static string BuildLogMessage(
           bool isLong,
           Dictionary<string, int> resolved,
           Dictionary<string, int> multiple,
           Dictionary<string, int> unresolved)
        {
            var longMessage = isLong ? "LONG" : "SHORT";
            longMessage += $" resolved:{resolved.Count} multiple:{multiple.Count} unresolved:{unresolved.Count}";
            return longMessage;
        }
    }
}
