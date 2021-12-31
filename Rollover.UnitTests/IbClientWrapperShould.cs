using AutoFixture.Xunit2;
using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Xunit;

namespace Rollover.UnitTests
{
    public class IbClientWrapperShould
    {
        [Theory, AutoNSubstituteData]
        public void CallEnqueueInOnContractDetails([Frozen] IIbClientQueue inputQueue)
        {
            var contractDetails = new ContractDetails();
            var contractDetailsMessage = new ContractDetailsMessage(5, contractDetails);

            var sut = new IbClientWrapper(inputQueue);
            sut.OnContractDetails(contractDetailsMessage);

            inputQueue.Received().Enqueue(Arg.Any<object>());
        }
    }
}
