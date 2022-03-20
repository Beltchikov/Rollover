using AutoFixture.Xunit2;
using NSubstitute;
using UsMoversOpening.Configuration;
using Xunit;

namespace UsMoversOpening.Tests
{
    public class UmoAgentShould
    {
        [Theory, AutoNSubstituteData]
        public void CallCofigurationManager(
            [Frozen] IConfigurationManager configurationManager,
            UmoAgent sut)
        {
            sut.Run();
            configurationManager.Received().GetConfiguration();
        }
    }
}
