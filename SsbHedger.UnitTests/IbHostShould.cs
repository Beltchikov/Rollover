using IbClient.messages;
using NSubstitute;
using SsbHedger.Utilities;
using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using AutoFixture.Xunit2;
using SsbHedger.IbModel;

namespace SsbHedger.UnitTests
{
    public class IbHostShould
    {
        [Fact]
        public void AddErrorMessageToViewModel()
        {
            int reqId = 1;
            int code = 2;
            string message = "3";
            Exception exception = new Exception("4");

            IConfiguration configuration = Substitute.For<IConfiguration>();
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            var sut = new IbHost(
                configuration,
                new PositionMessageBuffer(),
                new AtmStrikeUtility(),
                new ContractSpy());
            MainWindowViewModel viewModel = (MainWindowViewModel)FormatterServices
                .GetUninitializedObject(typeof(MainWindowViewModel));
            viewModel.Messages = new ObservableCollection<Message>();
            sut.ViewModel = viewModel;

            Reflection.CallMethod(
                sut,
                "_ibClient_Error",
                new object[] { reqId, code, message, exception });

            Assert.Single(viewModel.Messages);
            Assert.Equal(reqId, viewModel.Messages.First().ReqId);
            Assert.Equal($"Code:{code} message:{message} exception:{exception}",
                viewModel.Messages.First().Body);
        }

        [Fact]
        public void AddManagedAccountsMessageToViewModel()
        {
            var accounts = "acc1 ,acc2";
            ManagedAccountsMessage message = new ManagedAccountsMessage(accounts);

            IConfiguration configuration = Substitute.For<IConfiguration>();
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            var sut = new IbHost(
                configuration,
                new PositionMessageBuffer(),
                new AtmStrikeUtility(),
                new ContractSpy());
            MainWindowViewModel viewModel = (MainWindowViewModel)FormatterServices
                .GetUninitializedObject(typeof(MainWindowViewModel));
            viewModel.Messages = new ObservableCollection<Message>();
            sut.ViewModel = viewModel;

            Reflection.CallMethod(
                sut,
                "_ibClient_ManagedAccounts",
                new object[] { message });

            Assert.Single(viewModel.Messages);
            Assert.Equal(0, viewModel.Messages.First().ReqId);
            var expectedBody = $"Managed accounts: " +
                $"{message.ManagedAccounts.Aggregate((r, n) => r + "," + n)}";
            Assert.Equal(expectedBody, viewModel.Messages.First().Body);
        }

        [Fact]
        public void AddConnectionStatusMessageToViewModel()
        {
            ConnectionStatusMessage message = new ConnectionStatusMessage(true);

            IConfiguration configuration = Substitute.For<IConfiguration>();
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            var sut = new IbHost(
                configuration,
                new PositionMessageBuffer(),
                new AtmStrikeUtility(),
                new ContractSpy());
            MainWindowViewModel viewModel = (MainWindowViewModel)FormatterServices
                .GetUninitializedObject(typeof(MainWindowViewModel));
            viewModel.Messages = new ObservableCollection<Message>();
            sut.ViewModel = viewModel;

            Reflection.CallMethod(
                sut,
                "_ibClient_NextValidId",
                new object[] { message });

            Assert.Single(viewModel.Messages);
            Assert.Equal(0, viewModel.Messages.First().ReqId);
            var expectedBody = "CONNECTED!";
            Assert.Equal(expectedBody, viewModel.Messages.First().Body);
            Assert.StartsWith(expectedBody, viewModel.ConnectionMessage);
            Assert.True(viewModel.Connected);
        }

        [Fact]
        public void AddConnectionStatusMessageToViewModelNegative()
        {
            ConnectionStatusMessage message = new ConnectionStatusMessage(false);

            IConfiguration configuration = Substitute.For<IConfiguration>();
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            var sut = new IbHost(
                configuration,
                new PositionMessageBuffer(),
                new AtmStrikeUtility(),
                new ContractSpy());
            MainWindowViewModel viewModel = (MainWindowViewModel)FormatterServices
                .GetUninitializedObject(typeof(MainWindowViewModel));
            viewModel.Messages = new ObservableCollection<Message>();
            sut.ViewModel = viewModel;

            Reflection.CallMethod(
                sut,
                "_ibClient_NextValidId",
                new object[] { message });

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
            [Frozen] IConfiguration configuration,
            IbHost sut)
        {
            var lastTradeDate = "221111";
            
            // Prepare
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            // Act
            var strikes = sut.GetStrikesSpy(underlyingPrice, lastTradeDate, numberOfStrikes);

            // Verify
            Assert.IsType<List<double>>(strikes);
            Assert.Equal(numberOfStrikes, strikes.Count());
        }

        [Theory, AutoNSubstituteData]
        public void ReturnSpyStrikesSortedAndUnique(
            double underlyingPrice,
            int numberOfStrikes,
            [Frozen] IConfiguration configuration,
            IbHost sut)
        {
            var lastTradeDate = "221111";

            // Prepare
            configuration.GetValue(Configuration.UNDERLYING_SYMBOL).Returns("SPY");

            // Act
            var strikes = sut.GetStrikesSpy(underlyingPrice, lastTradeDate, numberOfStrikes).ToList();

            // Verify
            // Check ASC sort
            var zipped = strikes.Zip(strikes.Skip(1), (r, n) => n - r);
            Assert.True(zipped.All(r => r >= 0));

            // Check uniqueness
            Assert.Equal(strikes.Distinct().Count(), strikes.Count());
        }

        //ReturnSpyStrikesCenteredAroundUnderlyingPrice

        //CallIsValidStrikeForEveryStrike

        // ExcludeNotValidStrikes

        // ThrowIfTooManyInvalidStrikes

    }
}
