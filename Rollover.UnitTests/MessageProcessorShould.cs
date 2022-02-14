using IBApi;
using IBSampleApp.messages;
using Rollover.Ib;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Rollover.Tests.Shared;

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

        [Theory, AutoNSubstituteData]
        public void ReturnMessageIfTypeString(MessageProcessor sut)
        {
            var testMessage = "id=-1 errorCode=321 msg=Error validating request.-'cw' : cause - Invalid";
            var result = sut.ConvertMessage(testMessage);
            Assert.Equal(testMessage, result.First());
        }

        [Theory, AutoNSubstituteData]
        public void ReturnConnectedIfTypeConnectionStatusMessage(MessageProcessor sut)
        {
            var testMessage = new ConnectionStatusMessage(true);
            var result = sut.ConvertMessage(testMessage);
            Assert.Equal("Connected.", result.First());
        }

        [Theory, AutoNSubstituteData]
        public void ReturnDisconnectedIfTypeConnectionStatusMessage(MessageProcessor sut)
        {
            var testMessage = new ConnectionStatusMessage(false);
            var result = sut.ConvertMessage(testMessage);
            Assert.Equal("Disconnected.", result.First());
        }

        [Theory, AutoNSubstituteData]
        public void ReturnAccountsIfTypeManagedAccountsMessage(MessageProcessor sut)
        {
            var testMessage = new ManagedAccountsMessage("GOOG\r\nMSFT");
            var result = sut.ConvertMessage(testMessage);
            Assert.Contains("GOOG", result.First());
            Assert.Contains("MSFT", result.First());
        }

        [Theory, AutoNSubstituteData]
        public void ReturnNullIfTypePositionMessageAndNoOnPositionEndString(MessageProcessor sut)
        {
            List<Contract> contracts = new List<Contract>
            {
                new() {LocalSymbol = "STX"},
                new() {LocalSymbol = "PRDO"}
            };

            List<PositionMessage> positionMessages = new List<PositionMessage>
            {
                new("account", contracts[0], 1, 1000),
                new("account", contracts[1], 2, 2000),
            };

            var result1 = sut.ConvertMessage(positionMessages[0]);
            var result2 = sut.ConvertMessage(positionMessages[1]);

            Assert.Empty(result1);
            Assert.Empty(result2);
        }
    }
}
