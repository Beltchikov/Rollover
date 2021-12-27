using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
using System.Linq;
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

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository);

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

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository);

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

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository);

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

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository);

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

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository);

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

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository);

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
            var sut = new InputProcessor(null, null, null, null);
            var result = sut.Convert(testInput);
            Assert.Equal(testInput,result.First());
        }
    }
}
