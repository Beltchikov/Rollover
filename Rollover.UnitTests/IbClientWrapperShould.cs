using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using System.Threading;
using Xunit;

namespace Rollover.UnitTests
{
    public class IbClientWrapperShould
    {
        [Fact]
        public void CallTrackedSymbolFactoryFromContractDetailsMessageInOnContractDetails()
        {
            SynchronizationContext synchronizationContext = new SynchronizationContext();
            var inputQueue = Substitute.For<IInputQueue>();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbolFactory = Substitute.For<ITrackedSymbolFactory>();

            var sut = new IbClientWrapper(
                synchronizationContext, 
                inputQueue, 
                portfolio, 
                trackedSymbolFactory);

            var contractDetails = new ContractDetails();
            var contractDetailsMessage = new ContractDetailsMessage(5, contractDetails);
            sut.OnContractDetails(contractDetailsMessage);

            trackedSymbolFactory.Received().FromContractDetailsMessage(contractDetailsMessage);
        }
    }
}
