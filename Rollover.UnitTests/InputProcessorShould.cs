using NSubstitute;
using Rollover.Input;
using Rollover.Tracking;
using System.Linq;
using AutoFixture.Xunit2;
using Xunit;

namespace Rollover.UnitTests
{
    public class InputProcessorShould
    {
        [Fact]
        public void HaveStateConnectedIfFirstRun()
        {
            var reducer = new Reducer();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var sut = new InputProcessor(reducer, portfolio, trackedSymbols);

            sut.Convert(null);
            Assert.True(sut.State == "Connected");
        }

        [Fact]
        public void ReturnEmptyArrayIfStateIsWaitingForSymbolAndInputIsNull()
        {
            string testInput = null;

            var reducer = Substitute.For<IReducer>();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var sut = new InputProcessor(reducer, portfolio, trackedSymbols);

            var resultList = sut.Convert(testInput);
            Assert.True(!resultList.Any());
        }

        [Fact]
        public void ReturnInputIfStateIsWaitingForSymbolAndInputContainsState()
        {
            string testInput = "STATE: Diagnostic message";

            var reducer = Substitute.For<IReducer>();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var sut = new InputProcessor(reducer, portfolio, trackedSymbols);

            var resultList = sut.Convert(testInput);

            Assert.True(resultList.Count() == 1);
            Assert.True(resultList.First() == testInput);
        }

        [Fact]
        public void ReturnInputIfStateIsWaitingForSymbolAndInputIsEnterSymbolToTrack()
        {
            string testInput = "Enter a symbol to track:";

            var reducer = Substitute.For<IReducer>();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var sut = new InputProcessor(reducer, portfolio, trackedSymbols);

            var resultList = sut.Convert(testInput);

            Assert.True(resultList.Count() == 1);
            Assert.True(resultList.First() == testInput);
        }

        [Theory, AutoNSubstituteData]
        public void CallsPortfolioPositionBySymbol(
            [Frozen] IPortfolio portfolio,
            InputProcessor sut)
        {
            string testInput = "DAX:";
            sut.Convert(testInput);
            portfolio.Received().PositionBySymbol(testInput);
        }

        //[Fact]
        //public void ReturnSymbolAddedIfStateIsWaitingForSymbolAndInputIsValidSymbol()
        //{
        //    string testInput = "DAX:";

        //    var portfolio = Substitute.For<IPortfolio>();
        //    portfolio.PositionBySymbol(testInput).Returns(new IBSampleApp.messages.PositionMessage(
        //        "account",
        //        new IBApi.Contract(),
        //        1,
        //        1000));

        //    var trackedSymbols = Substitute.For<ITrackedSymbols>();
        //    trackedSymbols.SymbolExists(testInput).Returns(false);

        //    var sut = new InputProcessor();
        //    var resultList = sut.Convert(testInput, portfolio, trackedSymbols);

        //    Assert.True(resultList.Count() == 2);
        //    Assert.Contains(testInput, resultList.First());
        //    Assert.Contains("Symbol", resultList.First());
        //    Assert.Contains("added", resultList.First());
        //}

        //[Fact]
        //public void ReturnUnknownSymbolIfStateIsWaitingForSymbolAndInputIsInvalidSymbol()
        //{
        //    string testInput = "DAX:";

        //    var portfolio = Substitute.For<IPortfolio>();
        //    portfolio.SymbolExists(testInput).Returns(false);

        //    var sut = new InputProcessor();
        //    var resultList = sut.Convert(testInput, portfolio, null);

        //    Assert.True(resultList.Count() == 1);
        //    Assert.Contains("Unknown symbol", resultList.First());
        //}

        //[Fact]
        //public void ReturnSymbolAlreadyTrackedIfStateIsWaitingForSymbolAndAlreadyTracked()
        //{
        //    string testInput = "DAX:";

        //    var portfolio = Substitute.For<IPortfolio>();
        //    portfolio.SymbolExists(testInput).Returns(true);

        //    var trackedSymbols = Substitute.For<ITrackedSymbols>();
        //    trackedSymbols.SymbolExists(testInput).Returns(true);

        //    var sut = new InputProcessor();
        //    var resultList = sut.Convert(testInput, portfolio, trackedSymbols);

        //    Assert.True(resultList.Count() == 1);
        //    Assert.Contains(testInput, resultList.First());
        //    Assert.Contains("Symbol", resultList.First());
        //    Assert.Contains("is already tracked", resultList.First());
        //}
    }
}
