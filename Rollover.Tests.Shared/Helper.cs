using IBApi;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Ib;
using Rollover.Tracking;
using System;

namespace Rollover.Tests.Shared
{
    public class Helper
    {
        public const string HOST = "localhost";
        public const int PORT = 4001;
        public const int TIMEOUT = 10000;
        public const int PRICE_REQUEST_INTERVAL_IN_SECONDS = 10;

        public static Contract DaxIndContract()
        {
            return new Contract
            {
                Symbol = "DAX",
                SecType = "IND",
                Currency = "EUR",
                Exchange = "DTB",
            };
        }

        public static IRepository RepositoryFactory()
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

        public static Contract DaxOptContract(string lastTradeDateOrContractMonth)
        {
            return new Contract
            {
                Symbol = "DAX",
                SecType = "OPT",
                Currency = "EUR",
                Exchange = "DTB",
                LastTradeDateOrContractMonth = lastTradeDateOrContractMonth
            };
        }

        public static IConfigurationManager ConfigurationManagerFactory()
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

        public static int RandomClientId()
        {
            Random rnd = new Random();
            return rnd.Next();
        }
    }
}
