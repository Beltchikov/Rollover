using AutoFixture.Xunit2;
using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace Rollover.UnitTests
{
    public class InputProcessorShould
    {
        [Fact]
        public void HaveStateWaitingForSymbolIfFirstRun()
        {
            var reducer = new Reducer();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var repository = Substitute.For<IRepository>();

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository, null);

            sut.Convert(null);
            Assert.True(sut.State == "WaitingForSymbol");
        }

        [Fact]
        public void ReturnSymbolNotValid()
        {
            string testInput = null;

            var reducer = Substitute.For<IReducer>();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var repository = Substitute.For<IRepository>();

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository, null);

            var resultList = sut.Convert(testInput);
            Assert.True(resultList.Any());
            Assert.Equal("Symbol is not valid.", resultList.First());
        }

        [Fact]
        public void CallsPortfolioPositionBySymbol()
        {
            var testSymbol = "Some input";

            var reducer = new Reducer();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var repository = Substitute.For<IRepository>();

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository, null);

            var contract = new Contract() { Symbol = testSymbol };
            var positionMessage = new PositionMessage("account", contract, 1, 1000);
            portfolio.PositionBySymbol(Arg.Any<string>()).Returns(positionMessage);

            sut.Convert(testSymbol);

            portfolio.Received().PositionBySymbol(testSymbol);
        }

        [Fact]
        public void CallsTrackedSymbolsSymbolExists()
        {
            var testSymbol = "MNQ";

            var reducer = new Reducer();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var repository = Substitute.For<IRepository>();

            trackedSymbols.Add(Arg.Any<ITrackedSymbol>()).Returns(true);

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository, null);

            var contract = new Contract() { Symbol = testSymbol };
            var positionMessage = new PositionMessage("account", contract, 1, 1000);
            portfolio.PositionBySymbol(Arg.Any<string>()).Returns(positionMessage);

            sut.Convert("Enter a symbol to track:");
            sut.Convert(testSymbol);

            trackedSymbols.Received().SymbolExists(testSymbol);
        }

        [Fact]
        public void ReturnSymbolDetailsCouldNotBeQueried()
        {
            var testSymbol = "MNQ";

            var reducer = new Reducer();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var repository = Substitute.For<IRepository>();

            repository.GetTrackedSymbol(Arg.Any<Contract>()).Returns(null as ITrackedSymbol);

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository, null);

            var contract = new Contract() { Symbol = testSymbol };
            var positionMessage = new PositionMessage("account", contract, 1, 1000);
            portfolio.PositionBySymbol(Arg.Any<string>()).Returns(positionMessage);

            var resultList = sut.Convert(testSymbol);

            Assert.Single(resultList);
            Assert.Equal("Symbol details could not be queried.", resultList.First());
        }

        [Fact]
        public void CallsRepositoryGetTrackedSymbol()
        {
            var reducer = new Reducer();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var repository = Substitute.For<IRepository>();

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository, null);

            var testSymbol = "MNQ";
            var contract = new Contract() { Symbol = testSymbol };
            var positionMessage = new PositionMessage("account", contract, 1, 1000);
            portfolio.PositionBySymbol(Arg.Any<string>()).Returns(positionMessage);
            trackedSymbols.SymbolExists(Arg.Any<string>()).Returns(false);

            sut.Convert("Enter a symbol to track:");
            sut.Convert(testSymbol);

            repository.Received().GetTrackedSymbol(Arg.Any<Contract>());
        }

        [Fact]
        public void ReturnInputIfInputContainsErrorCode()
        {
            var testInput = "id=1 errorCode=321 msg=Error validating request.-'cw' : cause - Invalid";
            var sut = new InputProcessor(null, null, null, null, null);
            var result = sut.Convert(testInput);
            Assert.Equal(testInput, result.First());
        }

        [Fact]
        public void ReturnEmptyIfInputContainsErrorCodeAndIdMinusOne()
        {
            var testInput = "id=-1 errorCode=321 msg=Error validating request.-'cw' : cause - Invalid";
            var sut = new InputProcessor(null, null, null, null, null);
            var result = sut.Convert(testInput);
            Assert.Empty(result);
        }

        [Fact]
        public void ReturnMessageIfTypeString()
        {
            var testMessage = "id=-1 errorCode=321 msg=Error validating request.-'cw' : cause - Invalid";
            var sut = new InputProcessor(null, null, null, null, null);
            var result = sut.ConvertMessage(testMessage);
            Assert.Equal(testMessage, result.First());
        }

        [Fact]
        public void ReturnConnectedIfTypeConnectionStatusMessage()
        {
            var testMessage = new ConnectionStatusMessage(true);
            var sut = new InputProcessor(null, null, null, null, null);
            var result = sut.ConvertMessage(testMessage);
            Assert.Equal("Connected.", result.First());
        }

        [Fact]
        public void ReturnDisconnectedIfTypeConnectionStatusMessage()
        {
            var testMessage = new ConnectionStatusMessage(false);
            var sut = new InputProcessor(null, null, null, null, null);
            var result = sut.ConvertMessage(testMessage);
            Assert.Equal("Disconnected.", result.First());
        }

        [Fact]
        public void ReturnAccountsIfTypeManagedAccountsMessage()
        {
            var testMessage = new ManagedAccountsMessage("GOOG\r\nMSFT");
            var sut = new InputProcessor(null, null, null, null, null);
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

            var sut = new InputProcessor(null, null, null, null, null);
            var result1 = sut.ConvertMessage(positionMessages[0]);
            var result2 = sut.ConvertMessage(positionMessages[1]);

            Assert.Empty(result1);
            Assert.Empty(result2);
        }

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

            var sut = new InputProcessor(null, null, null, null, null);
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

            var sut = new InputProcessor(null, null, null, null, null);
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

            var sut = new InputProcessor(null, null, null, null, null);
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

            var sut = new InputProcessor(null, null, null, null, trackedSymbolFactory);
            var result = sut.ConvertMessage(message);

            var trackesSymbol = JsonSerializer.Deserialize<TrackedSymbol>(result.First());
            Assert.IsType<TrackedSymbol>(trackesSymbol);
        }
    }
}
