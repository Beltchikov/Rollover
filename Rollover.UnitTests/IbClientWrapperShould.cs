using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Tracking;
using Xunit;

namespace Rollover.UnitTests
{
    public class IbClientWrapperShould
    {
        [Fact]
        public void CallEnqueueInOnContractDetails()
        {
            var inputQueue = Substitute.For<IIbClientQueue>();
           
            var sut = new IbClientWrapper(inputQueue);

            var contractDetails = new ContractDetails();
            var contractDetailsMessage = new ContractDetailsMessage(5, contractDetails);
            sut.OnContractDetails(contractDetailsMessage);

            inputQueue.Received().Enqueue(Arg.Any<object>());
        }
    }
}
