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
                .Returns(new List<string>{"Some message"});

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

        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorReqMktData(
            [Frozen] IMessageCollector messageCollector,
            [Frozen] ITrackedSymbolFactory trackedSymbolFactory,
            [Frozen] Contract contract)
        {
            contract.LastTradeDateOrContractMonth = "02";
            contract.Symbol = "MNQ";
            contract.Exchange = "SMART";
            contract.SecType = "OPT";

            var contractDetails = new ContractDetails 
                { UnderSecType = "FUT", Contract = contract};
            var contractDetailsMessageList = new List<ContractDetailsMessage>
            { new ContractDetailsMessage(1, contractDetails)};
            messageCollector.reqContractDetails(Arg.Any<Contract>())
                .Returns(contractDetailsMessageList);

            var expirations = new HashSet<string> { "01", "02", "03" };
            var strikes = new HashSet<double>();
            var secDefOptParamMessage = new SecurityDefinitionOptionParameterMessage(
                Arg.Any<int>(),Arg.Any<string>(),Arg.Any<int>(),
                Arg.Any<string>(),Arg.Any<string>(),expirations, strikes);
            var secDefOptParamMessageList = new List<SecurityDefinitionOptionParameterMessage>
            { secDefOptParamMessage};
            messageCollector.reqSecDefOptParams(
                "MNQ", 
                "SMART", 
                "IND", 
                Arg.Any<int>()).Returns(secDefOptParamMessageList);   

            IRepository sut = new Repository(null, null, messageCollector);
            sut.GetTrackedSymbol(contract);
            messageCollector.Received().reqMktData(Arg.Any<Contract>(), "", true, false, null);
        }

        [Theory, AutoNSubstituteData]
        public void CallMessageCollectorReqSecDefOptParams(
            [Frozen] IMessageCollector messageCollector,
            [Frozen] ITrackedSymbolFactory trackedSymbolFactory)
        {
            var contract = new Contract
            {
                Symbol = "MNQ",
                Currency = "USD",
                Exchange = "GLOBEX",
                LastTradeDateOrContractMonth = "20220318",
                SecType ="OPT"
            };
            var contractDetails = new ContractDetails
            { Contract = contract, UnderSecType = "FUT" };
            contractDetails.Contract = contract;
            var contractDetailsMessage = new ContractDetailsMessage(1, contractDetails);
            var contractDetailsMessageList = new List<ContractDetailsMessage> { contractDetailsMessage };
            messageCollector.reqContractDetails(Arg.Any<Contract>()).Returns(contractDetailsMessageList);

            var expirations = new HashSet<string> { "20220318" };
            var strikes = new HashSet<double> { 980, 90, 100, 110, 120 };
            var secDefOptParamMessageList = new List<SecurityDefinitionOptionParameterMessage> {
            new SecurityDefinitionOptionParameterMessage(1,"",123, "", "", expirations, strikes)
            };
            messageCollector.reqSecDefOptParams(
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>())
                .Returns(secDefOptParamMessageList);

            IRepository sut = new Repository(null, null, messageCollector);
            sut.GetTrackedSymbol(contract);
            messageCollector.Received().reqSecDefOptParams(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<int>());
        }
    }
}
