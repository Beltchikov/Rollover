using IBApi;
using IBSampleApp.messages;
using Rollover.Tracking;
using Xunit;

namespace Rollover.UnitTests
{
    public class TrackedSymbolFactoryShould
    {
        [Fact]
        public void CreateTrackedSymbolFromContractDetailsMessage()
        {
            var requestId = 101;
            var contractDetails = new ContractDetails
            {
                Contract = new Contract
                {
                    Symbol = "MNQ",
                    ConId = 201,
                    SecType = "FOP",
                    Currency = "USD",
                    Exchange = "GLOBEX",
                    Strike = 1600
                }
            };
            var contractDetailsMessage = new ContractDetailsMessage(requestId, contractDetails);

            var sut = new TrackedSymbolFactory();
            var trackedSymbol = sut.FromContractDetailsMessage(contractDetailsMessage);

            Assert.Equal(contractDetailsMessage.RequestId, trackedSymbol.ReqIdContractDetails);
            Assert.Equal(contractDetailsMessage.ContractDetails.Contract.Symbol, trackedSymbol.Symbol);
            Assert.Equal(contractDetailsMessage.ContractDetails.Contract.ConId, trackedSymbol.ConId);
            Assert.Equal(contractDetailsMessage.ContractDetails.Contract.SecType, trackedSymbol.SecType);
            Assert.Equal(contractDetailsMessage.ContractDetails.Contract.Currency, trackedSymbol.Currency);
            Assert.Equal(contractDetailsMessage.ContractDetails.Contract.Exchange, trackedSymbol.Exchange);
            Assert.Equal(contractDetailsMessage.ContractDetails.Contract.Strike, trackedSymbol.Strike);
        }
    }
}
