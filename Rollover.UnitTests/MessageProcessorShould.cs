using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Rollover.UnitTests
{
    public class MessageProcessorShould
    {
        [Fact]
        public void ReturnLocalSymbolsAndEnterSymbolToTrackIfTypePositionMessage()
        {
            List<Contract> contracts = new List<Contract>
            {
                new Contract {LocalSymbol = "STX"},
                new Contract {LocalSymbol = "PRDO"}
            };

            List<PositionMessage> positionMessages = new List<PositionMessage>
            {
                new PositionMessage("account", contracts[0], 1, 1000),
                new PositionMessage("account", contracts[1], 2, 2000),
            };

            var sut = new MessageProcessor(null, null);
            sut.ConvertMessage(positionMessages[0]);
            sut.ConvertMessage(positionMessages[1]);
            var resultList = sut.ConvertMessage(Constants.ON_POSITION_END);

            Assert.Equal("PRDO", resultList[0]);
            Assert.Equal("STX", resultList[1]);
            Assert.Equal(Constants.ENTER_SYMBOL_TO_TRACK, resultList[2]);
        }

        [Fact]
        public void NotReturnOldLocalSymbolsIfTypePositionMessage()
        {
            List<Contract> contracts = new List<Contract>
            {
                new Contract {LocalSymbol = "STX"},
                new Contract {LocalSymbol = "PRDO"}
            };
            List<PositionMessage> positionMessages = new List<PositionMessage>
            {
                new PositionMessage("account", contracts[0], 1, 1000),
                new PositionMessage("account", contracts[1], 2, 2000),
            };

            var sut = new MessageProcessor(null, null);
            sut.ConvertMessage(positionMessages[0]);
            sut.ConvertMessage(positionMessages[1]);
            sut.ConvertMessage(Constants.ON_POSITION_END);

            // Second portion of messages
            contracts = new List<Contract>
            {
                new Contract {LocalSymbol = "CAG"},
                new Contract {LocalSymbol = "AA"}
            };
            positionMessages = new List<PositionMessage>
            {
                new PositionMessage("account", contracts[0], 1, 1000),
                new PositionMessage("account", contracts[1], 2, 2000),
            };

            sut.ConvertMessage(positionMessages[0]);
            sut.ConvertMessage(positionMessages[1]);
            var resultList = sut.ConvertMessage(Constants.ON_POSITION_END);

            Assert.Equal("AA", resultList[0]);
            Assert.Equal("CAG", resultList[1]);
            Assert.Equal(Constants.ENTER_SYMBOL_TO_TRACK, resultList[2]);
        }

        [Fact]
        public void IgnorePositionZeroIfTypePositionMessage()
        {
            List<Contract> contracts = new List<Contract>
            {
                new Contract {LocalSymbol = "STX"},
                new Contract {LocalSymbol = "PRDO"}
            };

            List<PositionMessage> positionMessages = new List<PositionMessage>
            {
                new PositionMessage("account", contracts[0], 1, 1000),
                new PositionMessage("account", contracts[1], 0, 2000),
            };

            var sut = new MessageProcessor(null, null);
            sut.ConvertMessage(positionMessages[0]);
            sut.ConvertMessage(positionMessages[1]);
            var resultList = sut.ConvertMessage(Constants.ON_POSITION_END);

            Assert.Equal("STX", resultList[0]);
            Assert.Equal(Constants.ENTER_SYMBOL_TO_TRACK, resultList[1]);
        }

        [Fact]
        public void ReturnSerializedTrackedSymbolIfTypeContractDetailsMessage()
        {
            var contractDetails = new ContractDetails();
            var message = new ContractDetailsMessage(1001, contractDetails);

            var trackedSymbolFactory = Substitute.For<ITrackedSymbolFactory>();
            trackedSymbolFactory.FromContractDetailsMessage(Arg.Any<ContractDetailsMessage>())
                .Returns(new TrackedSymbol());

            var sut = new MessageProcessor(null, null);
            var result = sut.ConvertMessage(message);

            var trackesSymbol = JsonSerializer.Deserialize<TrackedSymbol>(result.First());
            Assert.IsType<TrackedSymbol>(trackesSymbol);
        }
    }
}
