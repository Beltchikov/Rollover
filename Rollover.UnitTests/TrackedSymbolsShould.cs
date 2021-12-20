using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Rollover.Tracking;
using Xunit;

namespace Rollover.UnitTests
{
    public class TrackedSymbolsShould
    {
        [Fact]
        public void CallTrackedSymbolFactory()
        {
            var symbol = "someSymbol";
            var factory = Substitute.For<ITrackedSymbolFactory>();

            var sut = new TrackedSymbols(factory);
            sut.Add(symbol);
            factory.Received().Create(symbol);
        }
    }
}
