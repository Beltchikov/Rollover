﻿using IBApi;
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
        public void ReturnInputIfStateIsConnected()
        {
            string testInput = null;

            var reducer = Substitute.For<IReducer>();
            var portfolio = Substitute.For<IPortfolio>();
            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            var repository = Substitute.For<IRepository>();

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository);

            var resultList = sut.Convert(testInput);
            Assert.True(resultList.Any());
            Assert.Equal(testInput, resultList.First());
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

            var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository);

            var contract = new Contract() { Symbol = testSymbol };
            var positionMessage = new PositionMessage("account", contract, 1, 1000);
            portfolio.PositionBySymbol(Arg.Any<string>()).Returns(positionMessage);

            sut.Convert("Enter a symbol to track:");
            sut.Convert(testSymbol);

            trackedSymbols.Received().SymbolExists(testSymbol);
        }

        //[Fact]
        //public void CallsrepositoryContractDetails()
        //{
        //    var testSymbol = "MNQ";

        //    var reducer = new Reducer();
        //    var portfolio = Substitute.For<IPortfolio>();
        //    var trackedSymbols = Substitute.For<ITrackedSymbols>();
        //    var repository = Substitute.For<IRepository>();

        //    var sut = new InputProcessor(reducer, portfolio, trackedSymbols, repository);

        //    var contract = new Contract() { Symbol = testSymbol };
        //    var positionMessage = new PositionMessage("account", contract, 1, 1000);
        //    portfolio.PositionBySymbol(Arg.Any<string>()).Returns(positionMessage);
        //    trackedSymbols.SymbolExists(Arg.Any<string>()).Returns(false);

        //    sut.Convert("Enter a symbol to track:");
        //    sut.Convert(testSymbol);

        //    repository.Received().ContractDetails(Arg.Any<int>(), Arg.Any<Contract>());
        //}

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
    }
}
