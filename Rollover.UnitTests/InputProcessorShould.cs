using AutoFixture;
using AutoFixture.AutoNSubstitute;
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
        public void ReturnInputIfInputContainsAccountsFound(InputProcessor sut)
        {
            var testInput = "Accounts found: xxxxxxxx";
            var result = sut.Convert(testInput);
            Assert.Equal(testInput, result.First());
        }

        [Theory, AutoNSubstituteData]
        public void ReturnInputIfInputContainsConnected(InputProcessor sut)
        {
            var testInput = "Connected xxxxxxxx";
            var result = sut.Convert(testInput);
            Assert.Equal(testInput, result.First());
        }

        [Theory]
        [InlineData("q")]
        [InlineData("Q")]
        public void ReturnInputIfInputIsQ(string input)
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization { ConfigureMembers = true });
            var sut = fixture.Create<InputProcessor>();
            var result = sut.Convert(input);
            Assert.Equal(input, result.First());
        }

        [Theory, AutoNSubstituteData]
        public void CallsPortfolioSummary([Frozen] IPortfolio portfolio, InputProcessor sut)
        {
            sut.Convert("p");
            portfolio.Received().Summary();

            sut.Convert("P");
            portfolio.Received().Summary();
        }

        [Theory, AutoNSubstituteData]
        public void CallsTrackedSymbolsSummary([Frozen] ITrackedSymbols trackedSymbols, InputProcessor sut)
        {
            sut.Convert("t");
            trackedSymbols.Received().Summary();

            sut.Convert("T");
            trackedSymbols.Received().Summary();
        }
    }
}
