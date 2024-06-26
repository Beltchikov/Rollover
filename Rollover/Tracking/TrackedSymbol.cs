﻿using IBApi;
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
        private readonly double _quantity;

        private ContractDetailsMessage _contractDetailsMessage;
        private HashSet<double> _strikes;

        public TrackedSymbol(string localSymbol, int conId, string exchange, double quantity)
        {
            _localSymbol = localSymbol;
            _conId = conId;
            _exchange = exchange;
            _quantity = quantity;
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

        public double Quantity
        {
            get
            {
                return _quantity;
            }
        }

        public void ResetCache()
        {
            _contractDetailsMessage = null;
            _strikes = null;
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

        public double NextStrike(IRepository repository, double currentStrike)
        {
            var allStrikesOverPrice = Strikes(repository).Where(s => s > currentStrike).ToList();
            if (!allStrikesOverPrice.Any())
            {
                return 0;
            }
            return allStrikesOverPrice.Min();
        }

        public double NextButOneStrike(IRepository repository, double currentStrike)
        {
            var allStrikesOverPrice = Strikes(repository).Where(s => s > currentStrike).ToList();
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

        public double PreviousStrike(IRepository repository, double currentStrike)
        {
            var allStrikesBelowPrice = Strikes(repository).Where(s => s < currentStrike).ToList();
            if (!allStrikesBelowPrice.Any())
            {
                return 0;
            }
            return allStrikesBelowPrice.Max();
        }

        public double PreviousButOneStrike(IRepository repository, double currentStrike)
        {
            var allStrikesBelowPrice = Strikes(repository).Where(s => s < currentStrike).ToList();
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
            if(_strikes == null)
            {
                _strikes = repository.GetStrikes(
                 new Contract { ConId = _conId, Exchange = _exchange },
                 LastTradeDateOrContractMonth(repository));
            }

            return _strikes;
        }

        public override string ToString()
        {
            var toString = $"LocalSymbol: {LocalSymbol}  ConId: {ConId}  Exchange:{Exchange}";
            
            // TODO inject repository after deriving new class?
            // PROBLEM - constructor for serialization
            //var lastUnderlyingPrice = LastUnderlyingPrice()
            //if()

            return toString;    
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

        public double LastUnderlyingPrice(IRepository repository)
        {
            var lastPriceResult = repository.LastPrice(_conId, _exchange);
            if(lastPriceResult.Success)
            {
                return lastPriceResult.Value;
            }

            var closePriceResult = repository.ClosePrice(_conId, _exchange);
            if (closePriceResult.Success)
            {
                return closePriceResult.Value;
            }

            throw new LastUnderlyingPriceException();
        }
    }
}
