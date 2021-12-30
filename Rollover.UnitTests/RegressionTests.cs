using IBApi;
using NSubstitute;
using Rollover.Configuration;
using Rollover.Tracking;
using Xunit;

namespace Rollover.UnitTests
{
    public class RegressionTests
    {
        [Fact]
        public void GetTrackedSymbol()
        {
            var timeout = 1000;
            Configuration.Configuration configuration = new Configuration.Configuration
            { Timeout = timeout };
            var configurationManager = Substitute.For<IConfigurationManager>();
            configurationManager.GetConfiguration().Returns(configuration);

            IRolloverAgentFactory rolloverAgentFactory = new RolloverAgentFactory(configurationManager);
            IRolloverAgent rolloverAgent = rolloverAgentFactory.CreateInstance();

            var contract = new Contract();

            contract.SecType = "FOP";
            contract.Symbol = "MNQ";
            contract.Currency = "USD";
            contract.Exchange = null;

            contract.LocalSymbol = "MNQH2 C1650";
            contract.Strike = 16500;
            contract.LastTradeDateOrContractMonth = "20220318";
            contract.ConId = 515971773;

            var sut = rolloverAgent.Repository;
            ITrackedSymbol trackedSymbol = sut.GetTrackedSymbol(contract);

            
        }
    }
}
