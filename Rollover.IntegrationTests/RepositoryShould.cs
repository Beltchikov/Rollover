using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Tracking;
using System;
using Xunit;

namespace Rollover.IntegrationTests
{
    public class RepositoryShould
    {
        private readonly string HOST = "localhost";
        private readonly int PORT = 4001;
        private readonly int CLIENT_ID = 1;
        private readonly int TIMEOUT = 10000;
        private readonly int PRICE_REQUEST_INTERVAL_IN_SECONDS = 10;

        //[Fact]
        //public void ConnectFast()
        //{
        //    var repository = RepositoryFactory();
        //    repository.Disconnect();
        //    var connected = repository.IsConnected();
        //    var connnectionTuple = repository.Connect(HOST, PORT, CLIENT_ID);

        //    Assert.True(connnectionTuple.Item1);
        //}

        private IRepository RepositoryFactory()
        {
            IConfigurationManager configurationManager = ConfigurationManagerFactory();

            IIbClientQueue ibClientQueue = new IbClientQueue();
            IPortfolio portfolio = new Portfolio();

            IIbClientWrapper ibClient = new IbClientWrapper(ibClientQueue);

            IConnectedCondition connectedCondition = new ConnectedCondition();
            IMessageProcessor messageProcessor = new MessageProcessor(portfolio);
            IMessageCollector messageCollector = new MessageCollector(
                ibClient,
                connectedCondition,
                ibClientQueue,
                configurationManager);
            IRepository repository = new Repository(
                ibClient,
                messageProcessor,
                messageCollector);

            return repository;
        }

        private IConfigurationManager ConfigurationManagerFactory()
        {
            var configuration = new Configuration.Configuration
            {
                Host = HOST,
                Port = PORT,
                ClientId = CLIENT_ID,
                Timeout = TIMEOUT,
                PriceRequestIntervalInSeconds = PRICE_REQUEST_INTERVAL_IN_SECONDS
            };

            var configurationManager = Substitute.For<IConfigurationManager>();
            configurationManager.GetConfiguration().Returns(configuration);

            return configurationManager;
        }
    }
}
