using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using IbClient;
using IbClient.messages;
using NSubstitute;
using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace SsbHedger.UnitTests
{
    public class IbHostShould
    {
        [Theory, AutoNSubstituteData]
        public void AddErrorMessageToViewModel(
            int reqId,
            int code,
            string message,
            Exception exception,
            [Frozen] IConfiguration configuration,
            IbHost sut)
        {
            // Prepare
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            MainWindowViewModel viewModel = (MainWindowViewModel)FormatterServices
                .GetUninitializedObject(typeof(MainWindowViewModel));
            viewModel.Messages = new ObservableCollection<Message>();
            sut.ViewModel = viewModel;

            // Act
            Reflection.CallMethod(
                sut,
                "_ibClient_Error",
                new object[] { reqId, code, message, exception });

            // Verify
            Assert.Single(viewModel.Messages);
            Assert.Equal(reqId, viewModel.Messages.First().ReqId);
            Assert.Equal($"Code:{code} message:{message} exception:{exception}",
                viewModel.Messages.First().Body);
        }

        [Theory, AutoNSubstituteData]
        public void AddManagedAccountsMessageToViewModel(
            [Frozen] IConfiguration configuration,
            IbHost sut)
        {
            // Prepare
            var accounts = "acc1 ,acc2";
            ManagedAccountsMessage message = new ManagedAccountsMessage(accounts);
            
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            MainWindowViewModel viewModel = (MainWindowViewModel)FormatterServices
                .GetUninitializedObject(typeof(MainWindowViewModel));
            viewModel.Messages = new ObservableCollection<Message>();
            sut.ViewModel = viewModel;

            // Act
            Reflection.CallMethod(
                sut,
                "_ibClient_ManagedAccounts",
                new object[] { message });

            // Verify
            Assert.Single(viewModel.Messages);
            Assert.Equal(0, viewModel.Messages.First().ReqId);
            var expectedBody = $"Managed accounts: " +
                $"{message.ManagedAccounts.Aggregate((r, n) => r + "," + n)}";
            Assert.Equal(expectedBody, viewModel.Messages.First().Body);
        }

        [Theory, AutoNSubstituteData]
        public void AddConnectionStatusMessageToViewModel(
            [Frozen] IConfiguration configuration,
            IbHost sut)
        {
            // Prepare
            ConnectionStatusMessage message = new ConnectionStatusMessage(true);
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            MainWindowViewModel viewModel = (MainWindowViewModel)FormatterServices
                .GetUninitializedObject(typeof(MainWindowViewModel));
            viewModel.Messages = new ObservableCollection<Message>();
            sut.ViewModel = viewModel;

            // Act
            Reflection.CallMethod(
                sut,
                "_ibClient_NextValidId",
                new object[] { message });

            // Verify
            Assert.Single(viewModel.Messages);
            Assert.Equal(0, viewModel.Messages.First().ReqId);
            var expectedBody = "CONNECTED!";
            Assert.Equal(expectedBody, viewModel.Messages.First().Body);
            Assert.StartsWith(expectedBody, viewModel.ConnectionMessage);
            Assert.True(viewModel.Connected);
        }

        [Theory, AutoNSubstituteData]
        public void AddConnectionStatusMessageToViewModelNegative(
            [Frozen] IConfiguration configuration,
            IbHost sut)
        {
            // Prepare
            ConnectionStatusMessage message = new ConnectionStatusMessage(false);
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            MainWindowViewModel viewModel = (MainWindowViewModel)FormatterServices
                .GetUninitializedObject(typeof(MainWindowViewModel));
            viewModel.Messages = new ObservableCollection<Message>();
            sut.ViewModel = viewModel;

            // Act
            Reflection.CallMethod(
                sut,
                "_ibClient_NextValidId",
                new object[] { message });

            // Verify
            Assert.Single(viewModel.Messages);
            Assert.Equal(0, viewModel.Messages.First().ReqId);
            var expectedBody = "NOT CONNECTED!";
            Assert.Equal(expectedBody, viewModel.Messages.First().Body);
            Assert.StartsWith(expectedBody, viewModel.ConnectionMessage);
            Assert.False(viewModel.Connected);
        }

        [Theory, AutoNSubstituteData]
        public void AddConnectionStatusMessageOnDisconnect(
            [Frozen] IConfiguration configuration,
            IbHost sut)
        {
            // Prepare
            ConnectionStatusMessage messageConnected = new ConnectionStatusMessage(true);
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            MainWindowViewModel viewModel = (MainWindowViewModel)FormatterServices
                .GetUninitializedObject(typeof(MainWindowViewModel));
            viewModel.Messages = new ObservableCollection<Message>();
            sut.ViewModel = viewModel;

            // Act
            Reflection.CallMethod(
                sut,
                "_ibClient_NextValidId",
                new object[] { messageConnected });
            Reflection.CallMethod(
                sut,
                "_ibClient_ConnectionClosed",
                new object[] { });

            // Verify
            Assert.Equal(2, viewModel.Messages.Count);
            Assert.Equal(0, viewModel.Messages.Last().ReqId);
            var expectedBody = "DISCONNECTED!";
            Assert.Equal(expectedBody, viewModel.Messages.Last().Body);
            Assert.StartsWith(expectedBody, viewModel.ConnectionMessage);
            Assert.False(viewModel.Connected);
        }

        [Theory, AutoNSubstituteData]
        public void ReturnCorrectNumberOfSpyStrikes(
            double underlyingPrice,
            int numberOfStrikes,
            double strikeStep,
            [Frozen] IConfiguration configuration,
            IbHost sut)
        {
            var lastTradeDate = "221111";
            
            // Prepare
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            // Act
            var strikes = sut.GetStrikesSpy(underlyingPrice, lastTradeDate, numberOfStrikes, strikeStep);

            // Verify
            Assert.IsType<List<double>>(strikes);
            Assert.Equal(numberOfStrikes, strikes.Count());
        }

        [Theory, AutoNSubstituteData]
        public void ReturnSpyStrikesSortedAndUnique(
            double underlyingPrice,
            int numberOfStrikes,
            double strikeStep,
            [Frozen] IConfiguration configuration,
            IbHost sut)
        {
            var lastTradeDate = "221111";

            // Prepare
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            // Act
            var strikes = sut.GetStrikesSpy(underlyingPrice, lastTradeDate, numberOfStrikes, strikeStep).ToList();

            // Verify
            // Check ASC sort
            var zipped = strikes.Zip(strikes.Skip(1), (r, n) => n - r);
            Assert.True(zipped.All(r => r >= 0));

            // Check uniqueness
            Assert.Equal(strikes.Distinct().Count(), strikes.Count());
        }

        [Theory]
        [InlineData(209.4, "221111", 4, 1, "208, 209, 210, 211")]
        public void ReturnSpyStrikesCorrectly(
            double underlyingPrice,
            string lastTradeDate,
            int numberOfStrikes,
            double strikeStep,
            string strikesString)
        {
            // Prepare
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });

            IConfiguration configuration = Substitute.For<IConfiguration>();
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");
            IbHost sut = fixture.Create<IbHost>();

            // Act
            var strikes = sut.GetStrikesSpy(underlyingPrice, lastTradeDate, numberOfStrikes, strikeStep).ToList();

            // Verify
            //var countAboveUnderlying = strikes.Where(r => r >= underlyingPrice).Count();
            //var countBelowUnderlying = strikes.Where(r => r < underlyingPrice).Count();
            //Assert.Equal(countAboveUnderlying, countBelowUnderlying);

            var expectedStrikes = strikesString.Split(new char[] { ',' }).Select(d => Convert.ToDouble(d, CultureInfo.InvariantCulture));
            Assert.True(expectedStrikes.SequenceEqual(strikes));

        }

        //[Theory, AutoNSubstituteData]
        //public void ReturnSpyStrikesCenteredAroundUnderlyingPriceOddNumberOfStrikes(
        //    [Frozen] IConfiguration configuration,
        //    IbHost sut)
        //{
        //    double underlyingPrice = 209.4;
        //    var lastTradeDate = "221111";
        //    int numberOfStrikes = 11;
        //    double strikeStep = 1;

        //    // Prepare
        //    configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

        //    // Act
        //    var strikes = sut.GetStrikesSpy(underlyingPrice, lastTradeDate, numberOfStrikes, strikeStep).ToList();

        //    // Verify
        //    var countAboveUnderlying = strikes.Where(r => r >= underlyingPrice).Count();
        //    var countBelowUnderlying = strikes.Where(r => r < underlyingPrice).Count();
        //    Assert.Equal(countAboveUnderlying, countBelowUnderlying + 1);
        //}

        //[Theory, AutoNSubstituteData]
        //public void ReturnSpyStrikesCenteredAroundUnderlyingPriceAtmEvenNumberOfStrikes(
        //    [Frozen] IConfiguration configuration,
        //    IbHost sut)
        //{
        //    double underlyingPrice = 210;
        //    var lastTradeDate = "221111";
        //    int numberOfStrikes = 10;
        //    double strikeStep = 1;

        //    // Prepare
        //    configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

        //    // Act
        //    var strikes = sut.GetStrikesSpy(underlyingPrice, lastTradeDate, numberOfStrikes, strikeStep).ToList();

        //    // Verify
        //    var countAboveUnderlying = strikes.Where(r => r >= underlyingPrice).Count();
        //    var countBelowUnderlying = strikes.Where(r => r < underlyingPrice).Count();
        //    Assert.Equal(countAboveUnderlying, countBelowUnderlying);
        //}

        //[Theory, AutoNSubstituteData]
        //public void ReturnSpyStrikesCenteredAroundUnderlyingPriceAtmOddNumberOfStrikes(
        //    [Frozen] IConfiguration configuration,
        //    IbHost sut)
        //{
        //    double underlyingPrice = 210;
        //    var lastTradeDate = "221111";
        //    int numberOfStrikes = 11;
        //    double strikeStep = 1;

        //    // Prepare
        //    configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

        //    // Act
        //    var strikes = sut.GetStrikesSpy(underlyingPrice, lastTradeDate, numberOfStrikes, strikeStep).ToList();

        //    // Verify
        //    var countAboveUnderlying = strikes.Where(r => r >= underlyingPrice).Count();
        //    var countBelowUnderlying = strikes.Where(r => r < underlyingPrice).Count();
        //    Assert.Equal(countAboveUnderlying , countBelowUnderlying + 1);
        //}

        [Theory, AutoNSubstituteData]
        public void CallIsValidStrikeForEveryStrikeFromGetStrikesSpy(
            [Frozen] IIBClient ibClient,
            [Frozen] IConfiguration configuration,
            IbHost sut)
        {
            double underlyingPrice = 210;
            var lastTradeDate = "221111";
            int numberOfStrikes = 11;
            double strikeStep = 1;

            // Prepare
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");
            Reflection.SetFiledValue(sut, "_ibClient", ibClient);

            // Act
            sut.GetStrikesSpy(underlyingPrice, lastTradeDate, numberOfStrikes, strikeStep).ToList();

            // Verify
            ibClient.Received(numberOfStrikes).IsValidStrike(
                "SPY",
                lastTradeDate,
                Arg.Any<double>());
        }



        // ExcludeNotValidStrikes

        // ThrowIfTooManyInvalidStrikes

    }
}
