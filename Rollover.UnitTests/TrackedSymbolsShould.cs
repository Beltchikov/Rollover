using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Helper;
using Rollover.Tracking;
using System.Collections.Generic;
using Xunit;

namespace Rollover.UnitTests
{
    public class TrackedSymbolsShould
    {
        [Theory, AutoNSubstituteData]
        public void CallFileExist([Frozen] IFileHelper fileHelper, TrackedSymbols sut)
        {
            sut = new TrackedSymbols(fileHelper, null);
            fileHelper.Received().FileExists(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallReadAllText([Frozen] IFileHelper fileHelper, TrackedSymbols sut)
        {
            sut = new TrackedSymbols(fileHelper, null);
            fileHelper.Received().ReadAllText(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallDeserialize(
            [Frozen] IFileHelper fileHelper,
            [Frozen] ISerializer serializer, 
            TrackedSymbols sut)
        {
            sut = new TrackedSymbols(fileHelper, serializer);
            serializer.Received().Deserialize<HashSet<ITrackedSymbol>>(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallSerialize(
            TrackedSymbol trackedSymbol,
            [Frozen] ISerializer serializer,
            TrackedSymbols sut)
        {
            sut.Add(trackedSymbol);
            serializer.Received().Serialize(Arg.Any<HashSet<ITrackedSymbol>>());
        }
    }
}
