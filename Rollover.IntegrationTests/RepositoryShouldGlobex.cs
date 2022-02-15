using IBApi;
using Rollover.Helper;
using System;
using System.Linq;
using Xunit;

namespace Rollover.IntegrationTests
{
    public class RepositoryShouldGlobex
    {
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

            var repository = Tests.Shared.Helper.RepositoryFactory();
            if (!repository.IsConnected())
            {
                repository.Connect(
                    Tests.Shared.Helper.HOST, 
                    Tests.Shared.Helper.PORT, 
                    Tests.Shared.Helper.RandomClientId());
            }
            var contractDetails = repository.ContractDetails(contract);
            repository.Disconnect();
            Assert.True(contractDetails.Any());
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

            var repository = Tests.Shared.Helper.RepositoryFactory();
            if (!repository.IsConnected())
            {
                repository.Connect(
                    Tests.Shared.Helper.HOST, 
                    Tests.Shared.Helper.PORT, 
                    Tests.Shared.Helper.RandomClientId());
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
        public void ReceiveLastPriceMnqFuture()
        {
            var lastTradeDateOrContractMonth
                = IbHelper.NextContractYearAndMonth(DateTime.Now.Year, DateTime.Now.Month, 3);

            var contract = Tests.Shared.Helper.MnqFutContract(lastTradeDateOrContractMonth);

            var repository = Tests.Shared.Helper.RepositoryFactory();
            if (!repository.IsConnected())
            {
                repository.Connect(Tests.Shared.Helper.HOST, 
                    Tests.Shared.Helper.PORT, 
                    Tests.Shared.Helper.RandomClientId());
            }
            var contractDetailsList = repository.ContractDetails(contract);
            Assert.NotEmpty(contractDetailsList);
            Assert.Single(contractDetailsList);

            if (!repository.MarketIsOpen(contractDetailsList.First(), Tests.Shared.Helper.TimeNowChicago))
            {
                return;
            }

            var conId = contractDetailsList.First().ContractDetails.Contract.ConId;
            var priceTuple = repository.LastPrice(conId, contract.Exchange);
            Assert.True(priceTuple.Item1);

            repository.Disconnect();
        }

    }
}
