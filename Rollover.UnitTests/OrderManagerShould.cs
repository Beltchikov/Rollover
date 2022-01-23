using AutoFixture.Xunit2;
using IBApi;
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


        //[Theory, AutoNSubstituteData]
        //public void NotCallPlaceBearSpread(
        //    TrackedSymbols trackedSymbols,
        //    [Frozen] IRepository repository,
        //    OrderManager sut)
        //{
        //    sut.RolloverIfNextStrike(trackedSymbols);
        //    repository.DidNotReceive().PlaceBearSpread(
        //        Arg.Any<Contract>(),
        //        Arg.Any<int>(),
        //        Arg.Any<int>());
        //}
    }
}
