﻿using AutoFixture.Xunit2;
using NSubstitute;
using Rollover.Helper;
using Rollover.Tests.Shared;
using Rollover.Tracking;
using System.Collections.Generic;
using Xunit;

namespace Rollover.UnitTests
{
    public class TrackedSymbolsShould
    {
        [Theory, AutoNSubstituteData]
        public void CallFileExist(
            [Frozen] IFileHelper fileHelper,
            [Frozen] ISerializer serializer,
            TrackedSymbols sut)
        {
            sut = new TrackedSymbols(fileHelper, serializer);
            fileHelper.Received().FileExists(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallReadAllText(
            [Frozen] IFileHelper fileHelper,
            [Frozen] ISerializer serializer,
            TrackedSymbols sut)
        {
            sut = new TrackedSymbols(fileHelper, serializer);
            fileHelper.Received().ReadAllText(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallDeserialize(
            [Frozen] IFileHelper fileHelper,
            [Frozen] ISerializer serializer, 
            TrackedSymbols sut)
        {
            fileHelper.FileExists(Arg.Any<string>()).Returns(true);
            sut = new TrackedSymbols(fileHelper, serializer);
            serializer.Received().Deserialize<HashSet<TrackedSymbol>>(Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void CallSerialize(
            TrackedSymbol trackedSymbol,
            [Frozen] ISerializer serializer,
            TrackedSymbols sut)
        {
            sut.Add(trackedSymbol);
            serializer.Received().Serialize(Arg.Any<HashSet<TrackedSymbol>>());
        }

        [Theory, AutoNSubstituteData]
        public void CallWriteAllText(
            TrackedSymbol trackedSymbol,
            [Frozen] IFileHelper fileHelper,
            TrackedSymbols sut)
        {
            sut.Add(trackedSymbol);
            fileHelper.Received().WriteAllText(Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
