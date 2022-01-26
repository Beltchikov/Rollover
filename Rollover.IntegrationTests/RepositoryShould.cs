using IBApi;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Tracking;
using System;
using System.Linq;
using Xunit;

namespace Rollover.IntegrationTests
{
    public class RepositoryShould
    {
        private static readonly string HOST = "localhost";
        private static readonly int PORT = 4001;
        private static readonly int CLIENT_ID = 1;
        private static readonly int TIMEOUT = 10000;
        private static readonly int PRICE_REQUEST_INTERVAL_IN_SECONDS = 10;

        //private static readonly IRepository _repository = RepositoryFactory();

        [Fact]
        public void ConnectFast()
        {
            var repository = RepositoryFactory();
            var connnectionTuple = repository.Connect(HOST, PORT, CLIENT_ID);
            Assert.True(connnectionTuple.Item1);
        }

        //[Fact]
        //public void ReceiveContractDetaillForDaxOptions()
        //{
        //    var contract = new Contract
        //    {
        //        Symbol = "DAX",
        //        SecType = "OPT",
        //        Currency = "EUR",
        //        Exchange = "DTB"
        //    };

        //    var repository = RepositoryFactory();
        //    if (repository.IsConnected())
        //    {
        //        repository.Connect(HOST, PORT, CLIENT_ID);
        //    }
        //    var contractDetails = repository.ContractDetails(contract);
        //    Assert.True(contractDetails.Any());
        //}


        private static IRepository RepositoryFactory()
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

        private static IConfigurationManager ConfigurationManagerFactory()
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
