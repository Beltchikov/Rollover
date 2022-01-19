using AutoFixture.Xunit2;
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
            sut.RolloverIfNextStrike(trackedSymbols);
            repository.Received().GetCurrentPrice(Arg.Any<int>(), (Arg.Any<string>()));
        }
    }
}
