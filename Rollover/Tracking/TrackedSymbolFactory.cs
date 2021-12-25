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

        public TrackedSymbol FromContractDetailsMessage(ContractDetailsMessage contractDetailsMessage)
        {
            var trackedSymbol = new TrackedSymbol
            {
                ReqIdContractDetails = contractDetailsMessage.RequestId,
                Symbol = contractDetailsMessage.ContractDetails.Contract.Symbol,
                ConId = contractDetailsMessage.ContractDetails.Contract.ConId,
                SecType = contractDetailsMessage.ContractDetails.Contract.SecType,
                Currency = contractDetailsMessage.ContractDetails.Contract.Currency,
                Exchange = contractDetailsMessage.ContractDetails.Contract.Exchange,
                Strike = contractDetailsMessage.ContractDetails.Contract.Strike
            };
            return trackedSymbol;
        }
    }
}
