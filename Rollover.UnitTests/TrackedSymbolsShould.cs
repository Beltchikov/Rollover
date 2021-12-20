using NSubstitute;
using Rollover.Tracking;
using Xunit;

namespace Rollover.UnitTests
{
    public class TrackedSymbolsShould
    {
        [Fact]
        public void CallRequestSenderInBeginAdd()
        {
            var symbol = "someSymbol";
            var requestSender = Substitute.For<Ib.IRequestSender>();

            var sut = new TrackedSymbols(requestSender);
            sut.BeginAdd(symbol);
            requestSender.Received().ReqSecDefOptParams(
                Arg.Any<int>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<int>());
        }
    }
}
