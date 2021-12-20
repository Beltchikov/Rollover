using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Rollover.Ib;
using Rollover.Tracking;
using Xunit;

namespace Rollover.UnitTests
{
    public class TrackedSymbolFactoryShould
    {

        [Fact]
        public void CallRequestSender()
        {
            var ibClient = Substitute.For<IIbClientWrapper>();
            var requestSender = Substitute.For<RequestSender>(ibClient);
            
            var sut = new TrackedSymbolFactory();
            sut.Create("someSymbol");

            requestSender.Received().ReqSecDefOptParams(
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<int>());
        }
    }
}
