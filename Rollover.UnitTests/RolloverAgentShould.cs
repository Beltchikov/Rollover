using System;
using AutoFixture.Xunit2;
using NSubstitute;
using Xunit;

namespace Rollover.UnitTests
{
    public class RolloverAgentShould
    {
        [Theory, AutoNSubstituteData]
        public void CallConfigurationManagerCheckConfiguration(
            [Frozen] IConfigurationManager configurationManager,
            RolloverAgent sut)
        {
            configurationManager.Received().CheckConfiguration();
        }
    }
}
