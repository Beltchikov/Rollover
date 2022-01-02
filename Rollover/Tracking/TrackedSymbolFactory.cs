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
        public TrackedSymbol Create(string symbol)
        {
            throw new NotImplementedException();
        }

        public TrackedSymbol InitFromContractDetailsMessage(ContractDetailsMessage contractDetailsMessage)
        {
            var trackedSymbol = new TrackedSymbol
            {
                ReqIdContractDetails = contractDetailsMessage.RequestId,
                LocalSymbol = contractDetailsMessage.ContractDetails.Contract.LocalSymbol,
                Symbol = contractDetailsMessage.ContractDetails.Contract.Symbol,
                ConId = contractDetailsMessage.ContractDetails.Contract.ConId,
                SecType = contractDetailsMessage.ContractDetails.Contract.SecType,
                Currency = contractDetailsMessage.ContractDetails.Contract.Currency,
                Exchange = contractDetailsMessage.ContractDetails.Contract.Exchange,
                Strike = contractDetailsMessage.ContractDetails.Contract.Strike
            };
            return trackedSymbol;
        }

        public TrackedSymbol InitFromContract(Contract contract)
        {
            var trackedSymbol = new TrackedSymbol
            {
                LocalSymbol = contract.LocalSymbol,
                Symbol = contract.Symbol,
                ConId = contract.ConId,
                SecType = contract.SecType,
                Currency = contract.Currency,
                Exchange = contract.Exchange,
                Strike = contract.Strike
            };
            return trackedSymbol;
        }

        public TrackedSymbol Create(Contract contract, HashSet<double> strikes, double currentPrice)
        {
            var trackedSymbol = new TrackedSymbol
            {
                LocalSymbol = contract.LocalSymbol,
                Symbol = contract.Symbol,
                ConId = contract.ConId,
                SecType = contract.SecType,
                Currency = contract.Currency,
                Exchange = contract.Exchange,
                Strike = contract.Strike,
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
            return allStrikesOverPrice.Min();
        }
        private double NextButOneStrike(HashSet<double> strikes, double currentPrice)
        {
            var allStrikesOverPrice = strikes.Where(s => s > currentPrice).ToList();
            var firstMin = allStrikesOverPrice.Min();
            if (allStrikesOverPrice.Remove(firstMin))
            {
                return allStrikesOverPrice.Min();
            }

            throw new ApplicationException("Unexpected");
        }
        private double PreviousStrike(HashSet<double> strikes, double currentPrice)
        {
            var allStrikesBelowPrice = strikes.Where(s => s < currentPrice).ToList();
            return allStrikesBelowPrice.Max();
        }
        private double PreviousButOneStrike(HashSet<double> strikes, double currentPrice)
        {
            var allStrikesBelowPrice = strikes.Where(s => s < currentPrice).ToList();
            var firstMax = allStrikesBelowPrice.Max();
            if (allStrikesBelowPrice.Remove(firstMax))
            {
                return allStrikesBelowPrice.Max();
            }

            throw new ApplicationException("Unexpected");
        }
    }
}
