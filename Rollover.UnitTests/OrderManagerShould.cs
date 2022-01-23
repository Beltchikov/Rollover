using AutoFixture.Xunit2;
using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rollover.UnitTests
{
    public class OrderManagerShould
    {
        [Theory, AutoNSubstituteData]
        public void CallGetCurrentPrice(
            TrackedSymbols trackedSymbols,
            [Frozen] IRepository repository,
            OrderManager sut)
        {
            Tuple<bool, double> priceTuple = new Tuple<bool, double>(true, 100);
            repository.GetCurrentPrice(Arg.Any<int>(), Arg.Any<string>()).Returns(priceTuple);
            sut.RolloverIfNextStrike(trackedSymbols);
            repository.Received().GetCurrentPrice(Arg.Any<int>(), Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void ThrowsNoMarketDataException(
            TrackedSymbols trackedSymbols,
            [Frozen] IRepository repository,
            OrderManager sut)
        {
            Tuple<bool, double> priceTuple = new Tuple<bool, double>(false, 100);
            repository.GetCurrentPrice(Arg.Any<int>(), Arg.Any<string>()).Returns(priceTuple);

            Assert.Throws<NoMarketDataException>(() => sut.RolloverIfNextStrike(trackedSymbols));
        }

        [Theory, AutoNSubstituteData]
        public void NotCallPlaceBearSpreadPriceBelowOrEqualNextStrike(
            TrackedSymbol trackedSymbol,
            TrackedSymbols trackedSymbols,
            [Frozen] IRepository repository,
            OrderManager sut)
        {
            double nextStrike = 110;
            double currentPrice = 110;
            double currentStrike = 100;
            
            Tuple<bool, double> priceTuple = new Tuple<bool, double>(true, currentPrice);
            repository.GetCurrentPrice(Arg.Any<int>(), Arg.Any<string>()).Returns(priceTuple);
            
            var strikes = new HashSet<double> { 90, currentStrike, nextStrike };
            repository.GetStrikes(Arg.Any<Contract>(), Arg.Any<string>()).Returns(strikes);

            var contract = new Contract { Strike = currentStrike };
            var contractDetails = new ContractDetails { Contract = contract };
            var contractDetailsMessages = new List<ContractDetailsMessage>
            { new ContractDetailsMessage(1, contractDetails)};
            repository.ContractDetails(Arg.Any<Contract>())
                .Returns(contractDetailsMessages);

            trackedSymbols.Add(trackedSymbol);

            sut.RolloverIfNextStrike(trackedSymbols);
            repository.DidNotReceive().PlaceBearSpread(
                Arg.Any<int>(),
                Arg.Any<string>());
        }
    }
}
