using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger.CommandHandler;
using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
using SsbHedger.Utilities;

namespace SsbHedger.UnitTests.CommandHandler
{
    public class FindStrikesCommandHandlerShould
    {
        [Theory]
        [AutoNSubstituteData]
        public void CallGetStrikes(
            string underlying,
            int dte,
            int numberOfStrikes,
            MainWindowViewModel viewModel,
            object[] parameters,
            [Frozen] IIbHost ibHost,
            [Frozen] IConfiguration configuration,
            [Frozen] ILastTradeDateConverter lastTradeDateConverter,
            FindStrikesCommandHandler sut)
        {
            string lastTradeDate = "20170120";

            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns(underlying);
            configuration.GetValue(Configuration.DTE).Returns(dte);
            configuration.GetValue(Configuration.NUMBER_OF_STRIKES).Returns(numberOfStrikes);

            lastTradeDateConverter.FromDateTime(DateTime.Now.AddDays(dte)).Returns(lastTradeDate);

            sut.Handle(viewModel, parameters);
            ibHost.Received().GetStrikes(underlying, lastTradeDate, numberOfStrikes);
        }
    }
}
