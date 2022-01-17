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
            [Frozen] ITrackedSymbols trackedSymbols,
            InputProcessor sut)
        {
            trackedSymbols.Any().Returns(false); 
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
            Assert.Equal("Symbol details could not be queried. trackedSymbol is null.", resultList.First());
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
    }
}
