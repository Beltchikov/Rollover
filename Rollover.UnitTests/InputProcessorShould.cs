using AutoFixture.Xunit2;
using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Rollover.UnitTests
{
    public class InputProcessorShould
    {
        [Theory, AutoNSubstituteData]
        public void ReturnEmptyArrayIfInputNull(InputProcessor sut)
        {
            var output = sut.Convert(null);
            Assert.Empty(output);
        }

        [Theory, AutoNSubstituteData]
        public void HaveStateWaitingForSymbolIfFirstRun(
            InputProcessor sut)
        {
            sut.Convert("Some input");
            Assert.True(sut.State == "WaitingForSymbol");
        }

        [Theory, AutoNSubstituteData]
        public void ReturnSymbolNotValid([Frozen] IPortfolio portfolio, InputProcessor sut)
        {
            string testInput = "Invalid Symbol";
            portfolio.PositionBySymbol(Arg.Any<string>()).Returns((PositionMessage)null);

            var resultList = sut.Convert(testInput);
            Assert.True(resultList.Any());
            Assert.Equal("Symbol is not valid.", resultList.First());
        }

        [Theory, AutoNSubstituteData]
        public void CallsPortfolioPositionBySymbol([Frozen] IPortfolio portfolio, InputProcessor sut)
        {
            var testSymbol = "Some input";

            var contract = new Contract() { Symbol = testSymbol };
            var positionMessage = new PositionMessage("account", contract, 1, 1000);
            portfolio.PositionBySymbol(Arg.Any<string>()).Returns(positionMessage);

            sut.Convert(testSymbol);

            portfolio.Received().PositionBySymbol(testSymbol);
        }

        [Theory, AutoNSubstituteData]
        public void ReturnSymbolDetailsCouldNotBeQueried(
            [Frozen] IPortfolio portfolio, 
            [Frozen] IRepository repository, 
            InputProcessor sut)
        {
            var testSymbol = "MNQ";
            repository.GetTrackedSymbol(Arg.Any<Contract>()).Returns(null as ITrackedSymbol);

            var contract = new Contract() { Symbol = testSymbol };
            var positionMessage = new PositionMessage("account", contract, 1, 1000);
            portfolio.PositionBySymbol(Arg.Any<string>()).Returns(positionMessage);

            var resultList = sut.Convert(testSymbol);

            Assert.Single(resultList);
            Assert.Equal("Symbol details could not be queried.", resultList.First());
        }

        [Theory, AutoNSubstituteData]
        public void CallsRepositoryGetTrackedSymbol(
            [Frozen] IPortfolio portfolio,
            [Frozen] IRepository repository,
            [Frozen] ITrackedSymbols trackedSymbols,
            InputProcessor sut)
        {
            var testSymbol = "MNQ";
            var contract = new Contract() { Symbol = testSymbol };
            var positionMessage = new PositionMessage("account", contract, 1, 1000);
            portfolio.PositionBySymbol(Arg.Any<string>()).Returns(positionMessage);
            trackedSymbols.SymbolExists(Arg.Any<string>()).Returns(false);

            sut.Convert("Enter a symbol to track:");
            sut.Convert(testSymbol);

            repository.Received().GetTrackedSymbol(Arg.Any<Contract>());
        }

        [Theory, AutoNSubstituteData]
        public void ReturnInputIfInputContainsErrorCode(InputProcessor sut)
        {
            var testInput = "id=1 errorCode=321 msg=Error validating request.-'cw' : cause - Invalid";
            var result = sut.Convert(testInput);
            Assert.Equal(testInput, result.First());
        }

        [Theory, AutoNSubstituteData]
        public void ReturnEmptyIfInputContainsErrorCodeAndIdMinusOne(InputProcessor sut)
        {
            var testInput = "id=-1 errorCode=321 msg=Error validating request.-'cw' : cause - Invalid";
            var result = sut.Convert(testInput);
            Assert.Empty(result);
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

        [Fact]
        public void ReturnNullIfTypePositionMessageAndNoOnPositionEndString()
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

            var sut = new MessageProcessor(null);
            var result1 = sut.ConvertMessage(positionMessages[0]);
            var result2 = sut.ConvertMessage(positionMessages[1]);

            Assert.Empty(result1);
            Assert.Empty(result2);
        }
    }
}
