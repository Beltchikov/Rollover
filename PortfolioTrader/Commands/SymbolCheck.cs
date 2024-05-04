using IBApi;
using IbClient.IbHost;
using IbClient.messages;
using PortfolioTrader.Model;
using PortfolioTrader.Repository;
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
        private static readonly IRepository _repository = new JsonRepository();

        public static async Task Run(IBuyModelVisitor visitor)
        {
            _visitor = visitor;

            await ResolveSymbolsAndLog(visitor, true);
            await ResolveSymbolsAndLog(visitor, false);

            visitor.SymbolsChecked = true;
            visitor.TwsMessageCollection?.Add("DONE! Check Symbols command executed.");
        }

        private static async Task ResolveSymbolsAndLog(IBuyModelVisitor visitor, bool isLong)
        {
            var symbolsAndScoreAsDictionary = isLong
                ? SymbolsAndScore.StringToDictionary(visitor.LongSymbolsAsString)
                : SymbolsAndScore.StringToDictionary(visitor.ShortSymbolsAsString);

            // ResolveSymbolsByRepository
            (Dictionary<string, Position> resolvedByRepository, Dictionary<string, int> unresolvedByRepository)
                = ResolveSymbolsByRepository(symbolsAndScoreAsDictionary);
            visitor.TwsMessageCollection?.Add($"{resolvedByRepository.Count} symbols resolved by JSON repository. {unresolvedByRepository} remaining.");

            // ResolveSymbolsByTws
            var startMessage = BuildResolveTwsStartMessage(isLong: isLong, unresolvedByRepository);
            visitor.TwsMessageCollection?.Add(startMessage);

            Dictionary<string, Position> resolvedByTws = null!;
            Dictionary<string, int> multiple = null!, unresolved = null!;
            await Task.Run(async () =>
                (resolvedByTws, multiple, unresolved)
                = await ResolveSymbolsByTws(unresolvedByRepository)
            );

            // Join resolvedByRepository and resolvedByTws
            Dictionary<string, Position> allResolved = [];
            foreach ( var key in symbolsAndScoreAsDictionary.Keys) 
            { 
                if(resolvedByRepository.ContainsKey(key)) allResolved.Add(key, resolvedByRepository[key]);  
                else if (resolvedByTws.ContainsKey(key)) allResolved.Add(key, resolvedByTws[key]);
            }

            // Assign results
            if (isLong)
            {
                visitor.LongSymbolsResolved = SymbolsAndScore.PositionDictionaryToString(allResolved);
                visitor.LongSymbolsMultiple = SymbolsAndScore.DictionaryToString(multiple);
                visitor.LongSymbolsUnresolved = SymbolsAndScore.DictionaryToString(unresolved);
            }
            else
            {
                visitor.ShortSymbolsResolved = SymbolsAndScore.PositionDictionaryToString(allResolved);
                visitor.ShortSymbolsMultiple = SymbolsAndScore.DictionaryToString(multiple);
                visitor.ShortSymbolsUnresolved = SymbolsAndScore.DictionaryToString(unresolved);
            }

            var endMessage = BuildResolveTwsEndMessage(isLong: isLong, resolvedByTws, multiple, unresolved);
            visitor.TwsMessageCollection?.Add(endMessage);
        }

        private static (Dictionary<string, Position> resolvedByRepository, Dictionary<string, int> unresolvedByRepository)
            ResolveSymbolsByRepository(Dictionary<string, int> symbolsAndScoreAsDictionary)
        {
            Dictionary<string, Position> symbolsResolved = new Dictionary<string, Position>();
            Dictionary<string, int> symbolsUnresolved = new Dictionary<string, int>();
            foreach (var kvp in symbolsAndScoreAsDictionary)
            {
                int? conId = null;
                if ((conId = _repository.GetContractId(kvp.Key)) != null)
                    symbolsResolved.Add(kvp.Key, new Position()
                    {
                        NetBms = kvp.Value,
                        ConId = conId,
                    });
                else symbolsUnresolved.Add(kvp.Key, kvp.Value);
            }

            return (symbolsResolved, symbolsUnresolved);
        }

        private static async Task<(Dictionary<string, Position>, Dictionary<string, int>, Dictionary<string, int>)>
            ResolveSymbolsByTws(Dictionary<string, int> symbolsAndScoreAsDictionary)
        {
            Dictionary<string, Position> symbolsResolved = new Dictionary<string, Position>();
            Dictionary<string, int> symbolsMultiple = new Dictionary<string, int>();
            Dictionary<string, int> symbolsUnresolved = new Dictionary<string, int>();

            foreach (var symbol in symbolsAndScoreAsDictionary.Keys)
            {
                SymbolSamplesMessage symbolSamplesMessage
                    = await _visitor.IbHost.RequestMatchingSymbolsAsync(symbol, _visitor.Timeout);
                if (symbolSamplesMessage == null)
                {
                    symbolsUnresolved.Add(symbol, symbolsAndScoreAsDictionary[symbol]);
                }
                else
                {
                    var contractDescriptions = symbolSamplesMessage.ContractDescriptions;
                    if (contractDescriptions.Count() == 1)
                    {
                        symbolsResolved.Add(symbol, new Position()
                        {
                            NetBms = symbolsAndScoreAsDictionary[symbol],
                            ConId = contractDescriptions.First().Contract.ConId
                        });
                    }
                    else if (contractDescriptions.Count() == 0)
                    {
                        symbolsUnresolved.Add(symbol, symbolsAndScoreAsDictionary[symbol]);
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
                                NetBms = symbolsAndScoreAsDictionary[symbol],
                                ConId = contractDescriptionsNarrowed.First().Contract.ConId
                            });
                        }
                        else symbolsMultiple.Add(symbol, symbolsAndScoreAsDictionary[symbol]);
                    }
                }

                await Task.Run(() => Thread.Sleep((int)Math.Round(_visitor.Timeout * 1.5)));
            }

            return (symbolsResolved, symbolsMultiple, symbolsUnresolved);
        }

        private static string BuildResolveTwsStartMessage(
          bool isLong,
          Dictionary<string, int> symbols)
        {
            string longOrShort = isLong ? " LONG" : " SHORT";
            var message = $"{symbols.Count()} {longOrShort} symbols to resolve.";
            return message;
        }

        private static string BuildResolveTwsEndMessage(
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
