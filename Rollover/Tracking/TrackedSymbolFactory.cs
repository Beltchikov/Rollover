using IBApi;
using IBSampleApp.messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rollover.Tracking
{
    public class TrackedSymbolFactory : ITrackedSymbolFactory
    {
        public TrackedSymbol Create(Contract underlyingContract, HashSet<double> strikes, double currentPrice)
        {
            var trackedSymbol = new TrackedSymbol
            {
                UnderlyingLocalSymbol = underlyingContract.LocalSymbol,
                UnderlyingSymbol = underlyingContract.Symbol,
                UnderlyingConId = underlyingContract.ConId,
                UnderlyingSecType = underlyingContract.SecType,
                UnderlyingCurrency = underlyingContract.Currency,
                UnderlyingExchange = underlyingContract.Exchange,
                Strike = underlyingContract.Strike,
                NextStrike = NextStrike(strikes, currentPrice),
                NextButOneStrike = NextButOneStrike(strikes, currentPrice),
                PreviousStrike = PreviousStrike(strikes, currentPrice),
                PreviousButOneStrike = PreviousButOneStrike(strikes, currentPrice)
            };

            return trackedSymbol;
        }
        private double NextStrike(HashSet<double> strikes, double currentPrice)
        {
            var allStrikesOverPrice = strikes.Where(s => s > currentPrice).ToList();
            if (!allStrikesOverPrice.Any())
            {
                return 0;
            }
            return allStrikesOverPrice.Min();
        }
        private double NextButOneStrike(HashSet<double> strikes, double currentPrice)
        {
            var allStrikesOverPrice = strikes.Where(s => s > currentPrice).ToList();
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
        public double PreviousStrike(HashSet<double> strikes, double currentPrice)
        {
            var allStrikesBelowPrice = strikes.Where(s => s < currentPrice).ToList();
            if (!allStrikesBelowPrice.Any())
            {
                return 0;
            }
            return allStrikesBelowPrice.Max();
        }
        private double PreviousButOneStrike(HashSet<double> strikes, double currentPrice)
        {
            var allStrikesBelowPrice = strikes.Where(s => s < currentPrice).ToList();
            if (!allStrikesBelowPrice.Any())
            {
                return 0;
            }

            var firstMax = allStrikesBelowPrice.Max();
            if (allStrikesBelowPrice.Remove(firstMax))
            {
                if(!allStrikesBelowPrice.Any())
                {
                    return 0;
                }
                return allStrikesBelowPrice.Max();
            }

            throw new ApplicationException("Unexpected");
        }
    }
}
