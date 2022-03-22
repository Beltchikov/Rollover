using AutoFixture.Xunit2;
using NSubstitute;
using UsMoversOpening.Configuration;
using Xunit;

namespace UsMoversOpening.Tests
{
    public class ThreadSpawnerShould
    {
        [Theory, AutoNSubstituteData]
        public void CallUmoAgentStart(
            [Frozen] IUmoAgent umoAgent,
            ThreadSpawner sut)
        {
            sut.Run();
            umoAgent.Received().Run(sut, Arg.Any<IThreadWrapper>());
        }

        [Theory, AutoNSubstituteData]
        public void CallCofigurationManager(
            [Frozen] IConfigurationManager configurationManager,
            ThreadSpawner sut)
        {
            sut.Run();
            configurationManager.Received().GetConfiguration();
        }

        [Theory, AutoNSubstituteData]
        public void CallIbClientEConnect(
            string host,
            int port,
            int clientId,
            [Frozen] IConfigurationManager configurationManager,
            [Frozen] IIbClientWrapper ibClientWrapper,
            ThreadSpawner sut)
        {
            var configuration = new Configuration.Configuration
            {
                Host = host,
                Port = port,
                ClientId = clientId
            };
            configurationManager.GetConfiguration().Returns(configuration);

            sut.Run();
            ibClientWrapper.Received().eConnect(
                configuration.Host, 
                configuration.Port,
                configuration.ClientId);
        }
    }
}
