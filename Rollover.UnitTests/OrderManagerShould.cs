using AutoFixture.Xunit2;
using IBApi;
using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Tracking;
using System;
using System.Collections.Generic;
using Xunit;
using Rollover.Tests.Shared;

namespace Rollover.UnitTests
{
    public class OrderManagerShould
    {
        [Theory, AutoNSubstituteData]
        public void CallGetCurrentPrice(
            TrackedSymbols trackedSymbols,
            [Frozen] IRepository repository,
            OrderManager sut)
        {
            Result<double> priceResult = new Result<double> { Value = 100, Success = true };
            repository.LastPrice(Arg.Any<int>(), Arg.Any<string>()).Returns(priceResult);
            sut.RolloverIfNextStrike(trackedSymbols);
            repository.Received().LastPrice(Arg.Any<int>(), Arg.Any<string>());
        }

        [Theory, AutoNSubstituteData]
        public void NotCallPlaceBearSpreadPriceBelowOrEqualNextStrike(
            TrackedSymbol trackedSymbol,
            TrackedSymbols trackedSymbols,
            [Frozen] IRepository repository,
            OrderManager sut)
        {
            double nextStrike = 110;
            double currentPrice = 110;
            double currentStrike = 100;

            Result<double> priceResult = new Result<double> { Value = 100, Success = true };
            repository.LastPrice(Arg.Any<int>(), Arg.Any<string>()).Returns(priceResult);
            
            var strikes = new HashSet<double> { 90, currentStrike, nextStrike };
            repository.GetStrikes(Arg.Any<Contract>(), Arg.Any<string>()).Returns(strikes);

            var contract = new Contract { Strike = currentStrike };
            var contractDetails = new ContractDetails { Contract = contract };
            var contractDetailsMessages = new List<ContractDetailsMessage>
            { new ContractDetailsMessage(1, contractDetails)};
            repository.ContractDetails(Arg.Any<Contract>())
                .Returns(contractDetailsMessages);

            trackedSymbols.Add(trackedSymbol);

            sut.RolloverIfNextStrike(trackedSymbols);
            repository.DidNotReceive().PlaceBearSpread(Arg.Any<ITrackedSymbol>());
        }

        [Theory, AutoNSubstituteData]
        public void NotCallPlaceBearSpreadPriceAboveNextStrikeAndCurrentStrike(
            TrackedSymbol trackedSymbol,
            TrackedSymbols trackedSymbols,
            [Frozen] IRepository repository,
            OrderManager sut)
        {
            double currentPrice = 101;
            double nextStrike = 100;
            double currentStrike = 100;

            Result<double> priceResult = new Result<double> { Value = 100, Success = true };
            repository.LastPrice(Arg.Any<int>(), Arg.Any<string>()).Returns(priceResult);

            var strikes = new HashSet<double> { 90, currentStrike, nextStrike };
            repository.GetStrikes(Arg.Any<Contract>(), Arg.Any<string>()).Returns(strikes);

            var contract = new Contract { Strike = currentStrike };
            var contractDetails = new ContractDetails { Contract = contract };
            var contractDetailsMessages = new List<ContractDetailsMessage>
            { new ContractDetailsMessage(1, contractDetails)};
            repository.ContractDetails(Arg.Any<Contract>())
                .Returns(contractDetailsMessages);

            trackedSymbols.Add(trackedSymbol);

            sut.RolloverIfNextStrike(trackedSymbols);
            repository.DidNotReceive().PlaceBearSpread(Arg.Any<ITrackedSymbol>());
        }


        [Theory, AutoNSubstituteData]
        public void CallPlaceBearSpreadPrice(
            TrackedSymbol trackedSymbol,
            TrackedSymbols trackedSymbols,
            [Frozen] IRepository repository,
            OrderManager sut)
        {
            double currentPrice = 111;
            double nextStrike = 110;
            double currentStrike = 100;

            Result<double> priceResult = new Result<double> { Value = currentPrice, Success = true };
            repository.LastPrice(Arg.Any<int>(), Arg.Any<string>()).Returns(priceResult);

            var strikes = new HashSet<double> { 90, currentStrike, nextStrike };
            repository.GetStrikes(Arg.Any<Contract>(), Arg.Any<string>()).Returns(strikes);

            var contract = new Contract { Strike = currentStrike };
            var contractDetails = new ContractDetails { Contract = contract };
            var contractDetailsMessages = new List<ContractDetailsMessage>
            { new ContractDetailsMessage(1, contractDetails)};
            repository.ContractDetails(Arg.Any<Contract>())
                .Returns(contractDetailsMessages);

            trackedSymbols.Add(trackedSymbol);

            sut.RolloverIfNextStrike(trackedSymbols);
            repository.Received().PlaceBearSpread(Arg.Any<ITrackedSymbol>());
        }
    }
}
