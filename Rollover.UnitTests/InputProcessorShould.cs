using NSubstitute;
using Rollover.Input;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rollover.UnitTests
{
    public class InputProcessorShould
    {
        [Fact]
        public void ReturnInputIfStateIsConnecting()
        {
            var testInput = "TEST";
            var sut = new InputProcessor();

            var resultList = sut.Convert(testInput, "Connecting", null, null);

            Assert.True(resultList.Count() == 1);
            Assert.True(resultList.First() == testInput);
        }

        [Fact]
        public void ReturnInputIfStateIsConnected()
        {
            var testInput = "TEST";
            var sut = new InputProcessor();

            var resultList = sut.Convert(testInput, "Connected", null, null);

            Assert.True(resultList.Count() == 1);
            Assert.True(resultList.First() == testInput);
        }

        [Fact]
        public void ReturnEmptyArrayIfStateIsWaitingForSymbolAndInputIsNull()
        {
            string testInput = null;
            var sut = new InputProcessor();

            var resultList = sut.Convert(testInput, "WaitingForSymbol", null, null);

            Assert.True(!resultList.Any());
        }

        [Fact]
        public void ReturnInputIfStateIsWaitingForSymbolAndInputContainsState()
        {
            string testInput = "STATE: Diagnostic message";
            var sut = new InputProcessor();

            var resultList = sut.Convert(testInput, "WaitingForSymbol", null, null);

            Assert.True(resultList.Count() == 1);
            Assert.True(resultList.First() == testInput);
        }

        [Fact]
        public void ReturnInputIfStateIsWaitingForSymbolAndInputIsEnterSymbolToTrack()
        {
            string testInput = "Enter a symbol to track:";
            var sut = new InputProcessor();

            var resultList = sut.Convert(testInput, "WaitingForSymbol", null, null);

            Assert.True(resultList.Count() == 1);
            Assert.True(resultList.First() == testInput);
        }

        [Fact]
        public void ReturnSymbolAddedIfStateIsWaitingForSymbolAndInputIsValidSymbolk()
        {
            string testInput = "DAX:";

            var portfolio = Substitute.For<IPortfolio>();
            portfolio.SymbolExists(testInput).Returns(true);

            var trackedSymbols = Substitute.For<ITrackedSymbols>();
            trackedSymbols.SymbolExists(testInput).Returns(false);

            var sut = new InputProcessor();
            var resultList = sut.Convert(testInput, "WaitingForSymbol", portfolio, trackedSymbols);

            Assert.True(resultList.Count() == 1);
            Assert.Contains(testInput, resultList.First());
            Assert.Contains("Symbol", resultList.First());
            Assert.Contains("added", resultList.First());
        }

        [Fact]
        public void ReturnUnknownSymbolIfStateIsWaitingForSymbolAndInputIsInvalidSymbolk()
        {
            string testInput = "DAX:";

            var portfolio = Substitute.For<IPortfolio>();
            portfolio.SymbolExists(testInput).Returns(false);

            var sut = new InputProcessor();
            var resultList = sut.Convert(testInput, "WaitingForSymbol", portfolio, null);

            Assert.True(resultList.Count() == 1);
            Assert.Contains("Unknown symbol", resultList.First());
        }
    }
}
