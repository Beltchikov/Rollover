using AutoFixture.Xunit2;
using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Tracking;
using System.Collections.Generic;
using Xunit;

namespace Rollover.UnitTests
{
    public class RepositoryShould
    {
        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorEConnectInConnect(
            [Frozen] IMessageCollector messageCollector,
            Repository sut)
        {
            string host = "host1";
            int port = 398;
            int clientId = 29;

            sut.Connect(host, port, clientId);
            messageCollector.Received().eConnect(host, port, clientId);
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientDisconnect(
            [Frozen] IIbClientWrapper ibClient,
            Repository sut)
        {
            sut.Disconnect();
            ibClient.Received().eDisconnect();
        }

        [Theory, AutoNSubstituteData]
        public void CallsMessageCollectorReqPositionsInAllPositions([Frozen] IMessageCollector messageCollector)
        {
            var sut = new Repository(null, null, messageCollector);
            sut.AllPositions();
            messageCollector.Received().reqPositions();
        }



        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorEConnect(
            [Frozen] IMessageCollector messageCollector,
            [Frozen] IMessageProcessor messageProcessor)
        {
            messageProcessor.ConvertMessage(Arg.Any<object>())
                .Returns(new List<string> { "Some message" });

            IRepository sut = new Repository(null, messageProcessor, messageCollector);
            sut.Connect("localhost", 4001, 1);
            messageCollector.Received().eConnect("localhost", 4001, 1);
        }

        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorReqPositions(
            [Frozen] IMessageCollector messageCollector)
        {
            IRepository sut = new Repository(null, null, messageCollector);
            sut.AllPositions();
            messageCollector.Received().reqPositions();
        }

        //[Theory, AutoNSubstituteData]
        //public void ThrowMultipleContractDetailsMessage(
        //    [Frozen] IMessageCollector messageCollector,
        //    Repository sut)
        //{
        //    var contract = new Contract
        //    {
        //        Symbol = "MNQ",
        //        Currency = "USD",
        //        Exchange = "GLOBEX",
        //        LastTradeDateOrContractMonth = "20220318",
        //        SecType = "OPT"
        //    };

        //    var contractDetails = new ContractDetails
        //    { Contract = contract, UnderSecType = "FUT" };
        //    contractDetails.Contract = contract;
        //    var contractDetailsMessage = new ContractDetailsMessage(1, contractDetails);
        //    var contractDetailsMessageList = new List<ContractDetailsMessage>
        //    { contractDetailsMessage, contractDetailsMessage };
        //    messageCollector.reqContractDetails(Arg.Any<Contract>()).Returns(contractDetailsMessageList);

        //    Assert.Throws<MultipleContractDetailsMessage>(
        //        () => sut.PlaceBearSpread(Arg.Any<ITrackedSymbol>()));
        //}
    }
}
