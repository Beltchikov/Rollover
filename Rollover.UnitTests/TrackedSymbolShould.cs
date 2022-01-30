using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rollover.UnitTests
{
    public class TrackedSymbolShould
    {
        [Theory, AutoNSubstituteData]
        public void ReturnNextStrike(TrackedSymbol sut)
        {
            double currentStrike = 100;
            double expectedNextStrike = 110;
            var strikes = new HashSet<double> { 80, 90, 100, 110, 120 };

            var contractDetails = new ContractDetails();
            contractDetails.Contract
                = new Contract { LastTradeDateOrContractMonth = "202203" };
            var contractDetailsMessage = new ContractDetailsMessage(1, contractDetails);
            List<ContractDetailsMessage> contractDetailsMessages
                = new List<ContractDetailsMessage> { contractDetailsMessage };

            var repository = Substitute.For<IRepository>();
            repository.GetStrikes(Arg.Any<Contract>(), Arg.Any<string>())
                .Returns(strikes);
            repository.ContractDetails(Arg.Any<Contract>())
                .Returns(contractDetailsMessages);

            var nextStrike = sut.NextStrike(repository, currentStrike);
            Assert.Equal(expectedNextStrike, nextStrike);
        }

        [Theory, AutoNSubstituteData]
        public void ReturnNextButOneStrike(TrackedSymbol sut)
        {
            double currentStrike = 100;
            double expectedNextStrike = 120;
            var strikes = new HashSet<double> { 80, 90, 100, 110, 120 };

            var contractDetails = new ContractDetails();
            contractDetails.Contract
                = new Contract { LastTradeDateOrContractMonth = "202203" };
            var contractDetailsMessage = new ContractDetailsMessage(1, contractDetails);
            List<ContractDetailsMessage> contractDetailsMessages
                = new List<ContractDetailsMessage> { contractDetailsMessage };

            var repository = Substitute.For<IRepository>();
            repository.GetStrikes(Arg.Any<Contract>(), Arg.Any<string>())
                .Returns(strikes);
            repository.ContractDetails(Arg.Any<Contract>())
                .Returns(contractDetailsMessages);

            var nextStrike = sut.NextButOneStrike(repository, currentStrike);
            Assert.Equal(expectedNextStrike, nextStrike);
        }

        [Theory, AutoNSubstituteData]
        public void ReturnPreviousStrike(TrackedSymbol sut)
        {
            double currentStrike = 100;
            double expectedPreviousStrike = 90;
            var strikes = new HashSet<double> { 80, 90, 100, 110, 120 };

            var contractDetails = new ContractDetails();
            contractDetails.Contract
                = new Contract { LastTradeDateOrContractMonth = "202203" };
            var contractDetailsMessage = new ContractDetailsMessage(1, contractDetails);
            List<ContractDetailsMessage> contractDetailsMessages
                = new List<ContractDetailsMessage> { contractDetailsMessage };

            var repository = Substitute.For<IRepository>();
            repository.GetStrikes(Arg.Any<Contract>(), Arg.Any<string>())
                .Returns(strikes);
            repository.ContractDetails(Arg.Any<Contract>())
                .Returns(contractDetailsMessages);

            var nextStrike = sut.PreviousStrike(repository, currentStrike);
            Assert.Equal(expectedPreviousStrike, nextStrike);
        }

        [Theory, AutoNSubstituteData]
        public void ReturnPreviousButOneStrike(TrackedSymbol sut)
        {
            double currentStrike = 100;
            double expectedPreviousStrike = 80;
            var strikes = new HashSet<double> { 80, 90, 100, 110, 120 };

            var contractDetails = new ContractDetails();
            contractDetails.Contract
                = new Contract { LastTradeDateOrContractMonth = "202203" };
            var contractDetailsMessage = new ContractDetailsMessage(1, contractDetails);
            List<ContractDetailsMessage> contractDetailsMessages
                = new List<ContractDetailsMessage> { contractDetailsMessage };

            var repository = Substitute.For<IRepository>();
            repository.GetStrikes(Arg.Any<Contract>(), Arg.Any<string>())
                .Returns(strikes);
            repository.ContractDetails(Arg.Any<Contract>())
                .Returns(contractDetailsMessages);

            var nextStrike = sut.PreviousButOneStrike(repository, currentStrike);
            Assert.Equal(expectedPreviousStrike, nextStrike);
        }

        [Theory, AutoNSubstituteData]
        public void ReturnLastUnderlyingPrice(TrackedSymbol sut)
        {
            double lastPrice = 100;
            var lastPriceTuple = new Tuple<bool, double>(true, lastPrice);
            
            var repository = Substitute.For<IRepository>();
            repository.LastPrice(Arg.Any<int>(), Arg.Any<string>())
                .Returns(lastPriceTuple);
            
            var lastPriceReceived = sut.LastUnderlyingPrice(repository);
            Assert.Equal(100, lastPriceReceived);
        }
    }
}
