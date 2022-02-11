using IBApi;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Helper;
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
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday
                || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                return;
            }

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
        public void ReceiveLastPriceDaxIndex()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday
                || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                return;
            }

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

        [Fact]
        public void ReceiveLastPriceMnqFuture()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday
                || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                return;
            }

            var exchange = "GLOBEX";
            var lastTradeDateOrContractMonth
                = IbHelper.NextContractYearAndMonth(DateTime.Now.Year, DateTime.Now.Month, 3);

            var contract = new Contract
            {
                Symbol = "MNQ",
                SecType = "FUT",
                Currency = "USD",
                Exchange = exchange,
                LastTradeDateOrContractMonth = lastTradeDateOrContractMonth
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

        [Fact]
        public void ReceiveOptionParametersDax()
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

            var contractDetailsList = repository.ContractDetails(contract);
            Assert.Single(contractDetailsList);

            var optionParameterMessageList = repository.OptionParameters(
                contract.Symbol,
                contract.Exchange,
                contract.SecType,
                contractDetailsList.First().ContractDetails.Contract.ConId);
            Assert.True(optionParameterMessageList.Any());

            repository.Disconnect();
        }

        [Fact]
        public void ReceiveOptionParametersMnq()
        {
            var exchange = "GLOBEX";
            var lastTradeDateOrContractMonth
                = IbHelper.NextContractYearAndMonth(DateTime.Now.Year, DateTime.Now.Month, 3);

            var contract = new Contract
            {
                Symbol = "MNQ",
                SecType = "FUT",
                Currency = "USD",
                Exchange = exchange,
                LastTradeDateOrContractMonth = lastTradeDateOrContractMonth
            };

            var repository = RepositoryFactory();
            if (!repository.IsConnected())
            {
                repository.Connect(HOST, PORT, RandomClientId());
            }

            var contractDetailsList = repository.ContractDetails(contract);
            Assert.Single(contractDetailsList);

            var optionParameterMessageList = repository.OptionParameters(
                contract.Symbol,
                contract.Exchange,
                contract.SecType,
                contractDetailsList.First().ContractDetails.Contract.ConId);
            Assert.True(optionParameterMessageList.Any());

            repository.Disconnect();
        }

        [Fact]
        public void CreateAndDeleteOptionOrderDax()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday
                || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                return;
            }

            var exchange = "DTB";
            var contractUnderlying = new Contract
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
            var contractDetailsUnderlying = repository.ContractDetails(contractUnderlying);
            Assert.True(contractDetailsUnderlying.Any());

            var conIdUnderlying = contractDetailsUnderlying.First().ContractDetails.Contract.ConId;
            var priceTupleUnderlying = repository.LastPrice(conIdUnderlying, exchange);
            Assert.True(priceTupleUnderlying.Item1);

            // Get call contracts for expiration in 3 months
            var month = DateTime.Now.Month + 3;
            var lastTradeDateOrContractMonth
                = IbHelper.NextContractYearAndMonth(DateTime.Now.Year, month, 3);
            var contractCall = new Contract
            {
                Symbol = "DAX",
                SecType = "OPT",
                Currency = "EUR",
                Exchange = exchange,
                LastTradeDateOrContractMonth = lastTradeDateOrContractMonth,
                Right = "C"
            };
            var contractDetailsCallList = repository.ContractDetails(contractCall);
            Assert.True(contractDetailsCallList.Any());

            // Get strike next but one to the price
            var strikes = contractDetailsCallList.Select(c => c.ContractDetails.Contract.Strike).ToList();
            Assert.NotEmpty(strikes);
            var strikesAbovePrice = strikes.Where(s => s > priceTupleUnderlying.Item2).ToList();
            Assert.NotEmpty(strikesAbovePrice);
            strikes.Remove(strikesAbovePrice.Min());
            var strike = strikesAbovePrice.Min();

            // Update call contract with price
            contractCall.Strike = strike;

            // Get price for the call contract
            var contractDetailsCallWithStrike = repository.ContractDetails(contractCall);
            Assert.Single(contractDetailsCallWithStrike);
            var conIdCall = contractDetailsCallWithStrike.First().ContractDetails.Contract.ConId;
            var priceTupleCall = repository.AskPrice(conIdCall, exchange);
            Assert.True(priceTupleCall.Item1);

            // Decrease price by 5% to prevent execution
            var orderPrice = Math.Round(priceTupleCall.Item2 * 0.95, 1);

            // Place Order
            Order orderCall = new Order
            {
                Action = "BUY",
                OrderType = "LMT",
                TotalQuantity = 1,
                LmtPrice = orderPrice
            };
            repository.PlaceOrder(contractCall, orderCall);

            repository.Disconnect();
        }

        [Fact]
        public void PlaceBearSpreadAndCancelPlaceBearSpreadDax()
        {
            var exchange = "DTB";
            var month = DateTime.Now.Month + 3;
            var lastTradeDateOrContractMonth
                = IbHelper.NextContractYearAndMonth(DateTime.Now.Year, month, 3);

            // Connect to repository
            var repository = RepositoryFactory();
            if (!repository.IsConnected())
            {
                repository.Connect(HOST, PORT, RandomClientId());
            }

            // Get underlying price
            var contractUnderlying = new Contract
            {
                Symbol = "DAX",
                SecType = "IND",
                Currency = "EUR",
                Exchange = exchange
            };

            var contractDetailsUnderlying = repository.ContractDetails(contractUnderlying);
            Assert.True(contractDetailsUnderlying.Any());

            var conId = contractDetailsUnderlying.First().ContractDetails.Contract.ConId;
            var priceTuple = repository.LastPrice(conId, exchange);
            Assert.True(priceTuple.Item1);

            // Create call contract
            var contractCall = new Contract
            {
                Symbol = "DAX",
                SecType = "OPT",
                Currency = "EUR",
                Exchange = exchange,
                TradingClass = "ODAX",
                LastTradeDateOrContractMonth = lastTradeDateOrContractMonth,
                Right = "C"
            };

            var contractDetailsListCall = repository.ContractDetails(contractCall)
                .Where(c => c.ContractDetails.Contract.Strike > priceTuple.Item2)
                .OrderBy(d => d.ContractDetails.Contract.Strike);
            Assert.NotEmpty(contractDetailsListCall);
            var contractDetailsCall = contractDetailsListCall.First();

            Order orderCall = new Order
            {
                Action = "BUY",
                OrderType = "MKT",
                TotalQuantity = 1
            };

            // TODO
            //repository.PlaceOrder(contractCall, orderCall);

            //ibClient.ClientSocket.placeOrder(_nextOrderId, contract, order);
            //ibClient.ClientSocket.reqIds(-1);

            // TODO check Portfolio position
            //var allPositions = repository.AllPositions();
            //Assert.Empty(allPositions.Where(p
            //    => p.Contract.LocalSymbol 
            //    == contractDetailsCall.ContractDetails.Contract.LocalSymbol));

            //
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
