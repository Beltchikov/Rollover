using IBApi;
using Rollover.Tracking;
using System.Collections.Generic;
using Xunit;

namespace Rollover.UnitTests
{
    public class TrackedSymbolFactoryShould
    {
        [Fact]
        public void CreateTrackedSymbolFromContract()
        {
            Contract contract = new Contract
            {
                Symbol = "MNQ",
                ConId = 201,
                SecType = "FOP",
                Currency = "USD",
                Exchange = "GLOBEX",
                Strike = 80
            };
            HashSet<double> strikes = new HashSet<double>
            { 10,20,30,40,50,60,70,80,90};
            double price = 50;

            var sut = new TrackedSymbolFactory();
            var trackedSymbol = sut.Create(contract, strikes, price);

            Assert.Equal(contract.Symbol, trackedSymbol.Symbol);
            Assert.Equal(contract.ConId, trackedSymbol.ConId);
            Assert.Equal(contract.SecType, trackedSymbol.SecType);
            Assert.Equal(contract.Currency, trackedSymbol.Currency);
            Assert.Equal(contract.Exchange, trackedSymbol.Exchange);
            Assert.Equal(contract.Strike, trackedSymbol.Strike);

            Assert.Equal(60, trackedSymbol.NextStrike);
            Assert.Equal(70, trackedSymbol.NextButOneStrike);

            Assert.Equal(40, trackedSymbol.PreviousStrike);
            Assert.Equal(30, trackedSymbol.PreviousButOneStrike);
        }
    }
}
