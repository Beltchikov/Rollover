using IBApi;
using IBSampleApp.messages;
using Rollover.Ib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rollover.Tracking
{
    public class TrackedSymbol : ITrackedSymbol
    {
        private readonly string _localSymbol;
        private readonly int _conId;
        private readonly string _exchange;

        private ContractDetailsMessage _contractDetailsMessage;

        public TrackedSymbol(string localSymbol, int conId, string exchange)
        {
            _localSymbol = localSymbol;
            _conId = conId;
            _exchange = exchange;
}

        public string LocalSymbol
        {
            get
            {
                return _localSymbol;
            }
        }

        public int ConId
        {
            get
            {
                return _conId;
            }
        }

        public string Exchange
        {
            get
            {
                return _exchange;
            }
        }

        public void ResetCache()
        {
            _contractDetailsMessage = null;
        }

        public string Symbol(IRepository repository)
        {
            return GetContractPropety(repository, m => m?.ContractDetails?.Contract?.Symbol);
        }

        public string SecType(IRepository repository)
        {
            return GetContractPropety(repository, m => m?.ContractDetails?.Contract?.SecType);
        }

        public string Currency(IRepository repository)
        {
            return GetContractPropety(repository, m => m?.ContractDetails?.Contract?.Currency);
        }

        public string Right(IRepository repository)
        {
            return GetContractPropety(repository, m => m?.ContractDetails?.Contract?.Right);
        }

        public string LastTradeDateOrContractMonth(IRepository repository)
        {
            return GetContractPropety(repository, m => m?.ContractDetails?.Contract?.LastTradeDateOrContractMonth);
        }

        public double? Strike(IRepository repository)
        {
            return GetContractPropety(repository, m => m?.ContractDetails?.Contract?.Strike);
        }

        public double NextStrike(IRepository repository, double currentPrice)
        {
            var allStrikesOverPrice = Strikes(repository).Where(s => s > currentPrice).ToList();
            if (!allStrikesOverPrice.Any())
            {
                return 0;
            }
            return allStrikesOverPrice.Min();
        }

        public double NextButOneStrike(IRepository repository, double currentPrice)
        {
            var allStrikesOverPrice = Strikes(repository).Where(s => s > currentPrice).ToList();
            if (!allStrikesOverPrice.Any())
            {
                return 0;
            }

            var firstMin = allStrikesOverPrice.Min();
            if (allStrikesOverPrice.Remove(firstMin))
            {
                if (!allStrikesOverPrice.Any())
                {
                    return 0;
                }
                return allStrikesOverPrice.Min();
            }

            throw new ApplicationException("Unexpected");
        }

        public double PreviousStrike(IRepository repository, double currentPrice)
        {
            var allStrikesBelowPrice = Strikes(repository).Where(s => s < currentPrice).ToList();
            if (!allStrikesBelowPrice.Any())
            {
                return 0;
            }
            return allStrikesBelowPrice.Max();
        }

        public double PreviousButOneStrike(IRepository repository, double currentPrice)
        {
            var allStrikesBelowPrice = Strikes(repository).Where(s => s < currentPrice).ToList();
            if (!allStrikesBelowPrice.Any())
            {
                return 0;
            }

            var firstMax = allStrikesBelowPrice.Max();
            if (allStrikesBelowPrice.Remove(firstMax))
            {
                if (!allStrikesBelowPrice.Any())
                {
                    return 0;
                }
                return allStrikesBelowPrice.Max();
            }

            throw new ApplicationException("Unexpected");
        }

        public HashSet<double> Strikes(IRepository repository)
        {
            return repository.GetStrikes(
                 new Contract { ConId = _conId, Exchange = _exchange },
                 LastTradeDateOrContractMonth(repository));
        }

        public override string ToString()
        {
            return $"LocalSymbol: {LocalSymbol}  ConId: {ConId}  Exchange:{Exchange}";
        }

        public bool Equals(ITrackedSymbol otherSymbol)
        {
            return ConId == otherSymbol.ConId && Exchange == otherSymbol.Exchange;
        }

        private T GetContractPropety<T>(IRepository repository, Func<ContractDetailsMessage, T> selector)
        {
            if (_contractDetailsMessage == null)
            {
                _contractDetailsMessage = repository.ContractDetails(
                    new Contract { ConId = _conId, Exchange = _exchange })
                    .FirstOrDefault();
            }

            return selector(_contractDetailsMessage);
        }
    }
}
