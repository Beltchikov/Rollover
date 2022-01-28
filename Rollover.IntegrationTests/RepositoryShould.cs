using IBApi;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Tracking;
using System;
using System.Diagnostics;
using System.Linq;
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
            repository.Disconnect();


            Assert.True(connnectionTuple.Item1);
            Assert.InRange(stopwatch.Elapsed.TotalMilliseconds, 0, 3000);
        }

        private int RandomClientId()
        {
            Random rnd = new Random();
            return rnd.Next();
        }

        [Fact]
        public void ReceiveContractDetailsForDaxOptions()
        {
            //Due to the potentially high amount of data resulting from such queries this request is subject to pacing.
            //Although a request such as the above one will be answered immediately, 
            //a similar subsequent one will be kept on hold for one minute. 
            //This amount of time will increase if more such requests are performed. 
            //To prevent this, narrow down the amount of eligible contracts by providing an expiration date 
            //specifying at least the year(i.e. 2016) or the year and the month(i.e. 201603 for March 2016).

            var contract = new Contract
            {
                Symbol = "DAX",
                SecType = "OPT",
                Currency = "EUR",
                Exchange = "DTB",
                LastTradeDateOrContractMonth = DateTime.Now.Year.ToString() + "03"
            };

            var repository = RepositoryFactory();
            if (!repository.IsConnected())
            {
                repository.Connect(HOST, PORT, RandomClientId());
            }
            var contractDetails = repository.ContractDetails(contract);
            repository.Disconnect();
            Assert.True(contractDetails.Any());
        }

        [Fact]
        public void ReceiveContractDetailsForMnqOptions()
        {
            var contract = new Contract
            {
                Symbol = "MNQ",
                SecType = "FOP",
                Currency = "USD",
                Exchange = "GLOBEX",
                LastTradeDateOrContractMonth = DateTime.Now.Year.ToString() + "03"
            };

            var repository = RepositoryFactory();
            if (!repository.IsConnected())
            {
                repository.Connect(HOST, PORT, RandomClientId());
            }
            var contractDetails = repository.ContractDetails(contract);
            repository.Disconnect();
            Assert.True(contractDetails.Any());
        }

        [Fact]
        public void ReceiveCurrentPriceDax()
        {
            var exchange = "DTB";
            var contract = new Contract
            {
                Symbol = "DAX",
                SecType = "IND",
                Currency = "EUR",
                Exchange = exchange,
            };

            var repository = RepositoryFactory();
            if (!repository.IsConnected())
            {
                repository.Connect(HOST, PORT, RandomClientId());
            }
            var contractDetails = repository.ContractDetails(contract);
            Assert.True(contractDetails.Any());

            var conId = contractDetails.First().ContractDetails.Contract.ConId;
            var priceTuple = repository.LastPrice(conId, exchange);
            Assert.True(priceTuple.Item1);

            repository.Disconnect();

        }


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
