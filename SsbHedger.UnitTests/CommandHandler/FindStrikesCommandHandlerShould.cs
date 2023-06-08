using AutoFixture.Xunit2;
using NSubstitute;
using SsbHedger.CommandHandler;
using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
using SsbHedger.Utilities;
using System.Globalization;

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
            DateTime lastTradeDateTime = DateTime.Parse("20.01.2017", new CultureInfo("DE-de"));

            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns(underlying);
            configuration.GetValue(Configuration.DTE).Returns(dte);
            configuration.GetValue(Configuration.NUMBER_OF_STRIKES).Returns(numberOfStrikes);

            lastTradeDateConverter.DateTimeFromDte(dte).Returns(lastTradeDateTime);
            lastTradeDateConverter.FromDateTime(lastTradeDateTime).Returns(lastTradeDate);


            sut.Handle(viewModel, parameters);
            ibHost.Received().GetStrikes(underlying, lastTradeDate, numberOfStrikes);
        }
    }
}
