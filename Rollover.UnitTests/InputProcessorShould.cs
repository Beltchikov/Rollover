using NSubstitute;
using Rollover.Input;
using Rollover.Tracking;
using System.Linq;
using AutoFixture.Xunit2;
using Xunit;
using IBSampleApp.messages;
using IBApi;
using Rollover.Ib;

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
            var requestSender = Substitute.For<IRepository>();

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols);

            sut.Convert(null, requestSender);
            Assert.True(sut.State == "WaitingForSymbol");
        }

        [Fact]
        public void ReturnInputIfStateIsConnected()
        {
            string testInput = null;

            var reducer = Substitute.For<IReducer>();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var requestSender = Substitute.For<IRepository>();

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols);

            var resultList = sut.Convert(testInput, requestSender);
            Assert.True(resultList.Any());
            Assert.Equal(testInput, resultList.First());
        }

        [Fact]
        public void ReturnInputIfStateIsWaitingForSymbolAndInputContainsState()
        {
            string testInput = "STATE: Diagnostic message";

            var reducer = Substitute.For<IReducer>();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var requestSender = Substitute.For<IRepository>();

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols);

            var resultList = sut.Convert(testInput, requestSender);

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
            var requestSender = Substitute.For<IRepository>();

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols);

            var resultList = sut.Convert(testInput, requestSender);

            Assert.True(resultList.Count() == 1);
            Assert.True(resultList.First() == testInput);
        }

        [Fact]
        public void TransitToStateWaitingForSymbol()
        {
            var reducer = new Reducer();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var requestSender = Substitute.For<IRepository>();

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols);

            sut.Convert("Enter a symbol to track:", requestSender);
            Assert.True(sut.State == "WaitingForSymbol");
        }

        [Fact]
        public void CallsPortfolioPositionBySymbol()
        {
            var testSymbol = "Some input";

            var reducer = new Reducer();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var requestSender = Substitute.For<IRepository>();

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols);

            var contract = new Contract() { Symbol = testSymbol };
            var positionMessage = new PositionMessage("account", contract, 1, 1000);
            portfolio.PositionBySymbol(Arg.Any<string>()).Returns(positionMessage);

            sut.Convert("Enter a symbol to track:", requestSender);
            sut.Convert(testSymbol, requestSender);
            
            portfolio.Received().PositionBySymbol(testSymbol);
        }

        [Fact]
        public void CallsTrackedSymbolsSymbolExists()
        {
            var testSymbol = "MNQ";

            var reducer = new Reducer();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var requestSender = Substitute.For<IRepository>();

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols);

            var contract = new Contract() { Symbol = testSymbol };
            var positionMessage = new PositionMessage("account", contract, 1, 1000);
            portfolio.PositionBySymbol(Arg.Any<string>()).Returns(positionMessage);

            sut.Convert("Enter a symbol to track:", requestSender);
            sut.Convert(testSymbol, requestSender);

            trackedSymbols.Received().SymbolExists(testSymbol);
        }

        [Fact]
        public void CallsRequestSenderContractDetails()
        {
            var testSymbol = "MNQ";

            var reducer = new Reducer();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var requestSender = Substitute.For<IRepository>();
                        
            var sut = new InputProcessor(reducer, portfolio, trackedSymbols);

            var contract = new Contract() { Symbol = testSymbol };
            var positionMessage = new PositionMessage("account", contract, 1, 1000);
            portfolio.PositionBySymbol(Arg.Any<string>()).Returns(positionMessage);
            trackedSymbols.SymbolExists(Arg.Any<string>()).Returns(false);

            sut.Convert("Enter a symbol to track:", requestSender);
            sut.Convert(testSymbol, requestSender);

            requestSender.Received().ContractDetails(Arg.Any<int>(), Arg.Any<Contract>());
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
