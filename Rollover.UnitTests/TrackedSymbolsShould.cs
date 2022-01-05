using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Helper;
using Rollover.Tracking;
using Xunit;

namespace Rollover.UnitTests
{
    public class TrackedSymbolsShould
    {
        [Theory, AutoNSubstituteData]
        public void CallFileExist([Frozen] IFileHelper fileHelper, TrackedSymbols sut)
        {
            sut = new TrackedSymbols(fileHelper);
            fileHelper.Received().FileExists(Arg.Any<string>());
        }
    }
}
