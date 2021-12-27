using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using Xunit;

namespace Rollover.UnitTests
{
    public class IbClientWrapperShould
    {
        [Fact]
        public void CallTrackedSymbolFactoryFromContractDetailsMessageInOnContractDetails()
        {
            var inputQueue = Substitute.For<IIbClientQueue>();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbolFactory = Substitute.For<ITrackedSymbolFactory>();

            var sut = new IbClientWrapper(
                inputQueue, 
                portfolio, 
                trackedSymbolFactory);

            var contractDetails = new ContractDetails();
            var contractDetailsMessage = new ContractDetailsMessage(5, contractDetails);
            sut.OnContractDetails(contractDetailsMessage);

            trackedSymbolFactory.Received().FromContractDetailsMessage(contractDetailsMessage);
        }


        [Fact]
        public void CallEnqueueInOnContractDetails()
        {
            var inputQueue = Substitute.For<IIbClientQueue>();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbolFactory = Substitute.For<ITrackedSymbolFactory>();

            var sut = new IbClientWrapper(
                inputQueue,
                portfolio,
                trackedSymbolFactory);

            var contractDetails = new ContractDetails();
            var contractDetailsMessage = new ContractDetailsMessage(5, contractDetails);
            sut.OnContractDetails(contractDetailsMessage);

            inputQueue.Received().Enqueue(Arg.Any<string>());
        }
    }
}
