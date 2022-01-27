using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Tracking;
using System;
using System.Diagnostics;
using Xunit;

namespace Rollover.IntegrationTests
{
    public class RepositoryShould
    {
        private static readonly string HOST = "localhost";
        private static readonly int PORT = 4001;
        private static readonly int TIMEOUT = 10000;
        private static readonly int PRICE_REQUEST_INTERVAL_IN_SECONDS = 10;

        [Fact]
        public void ConnectFast()
        {
            var repository = RepositoryFactory();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var connnectionTuple = repository.Connect(HOST, PORT, RandomClientId());
            
            Assert.True(connnectionTuple.Item1);
            Assert.InRange(stopwatch.Elapsed.TotalMilliseconds, 0, 3000);
        }

        private int RandomClientId()
        {
            Random rnd = new Random();
            return rnd.Next();
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
                ClientId = RandomClientId(),
                Timeout = TIMEOUT,
                PriceRequestIntervalInSeconds = PRICE_REQUEST_INTERVAL_IN_SECONDS
            };

            var configurationManager = Substitute.For<IConfigurationManager>();
            configurationManager.GetConfiguration().Returns(configuration);

            return configurationManager;
        }
    }
}
