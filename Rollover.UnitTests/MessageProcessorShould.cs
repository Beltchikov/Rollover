using IBApi;
using IBSampleApp.messages;
using Rollover.Ib;
using System.Collections.Generic;
using Xunit;

namespace Rollover.UnitTests
{
    public class MessageProcessorShould
    {
        [Theory, AutoNSubstituteData]
        public void ReturnLocalSymbolsAndEnterSymbolToTrackIfTypePositionMessage(
            MessageProcessor sut)
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

            sut.ConvertMessage(positionMessages[0]);
            sut.ConvertMessage(positionMessages[1]);
            var resultList = sut.ConvertMessage(Constants.ON_POSITION_END);

            Assert.Equal("PRDO", resultList[0]);
            Assert.Equal("STX", resultList[1]);
            Assert.Equal(Constants.ENTER_SYMBOL_TO_TRACK, resultList[2]);
        }

        [Theory, AutoNSubstituteData]
        public void NotReturnOldLocalSymbolsIfTypePositionMessage(MessageProcessor sut)
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

        [Theory, AutoNSubstituteData]
        public void IgnorePositionZeroIfTypePositionMessage(MessageProcessor sut)
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

            sut.ConvertMessage(positionMessages[0]);
            sut.ConvertMessage(positionMessages[1]);
            var resultList = sut.ConvertMessage(Constants.ON_POSITION_END);

            Assert.Equal("STX", resultList[0]);
            Assert.Equal(Constants.ENTER_SYMBOL_TO_TRACK, resultList[1]);
        }
    }
}
