using IbClient.messages;
using NSubstitute;
using SsbHedger.Model;
using SsbHedger.SsbConfiguration;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

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
            var sut = new IbHost(configuration);
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
            var sut = new IbHost(configuration);
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
            Assert.Equal(expectedBody,viewModel.Messages.First().Body);
        }

        [Fact]
        public void AddConnectionStatusMessageToViewModel()
        {
            ConnectionStatusMessage message = new ConnectionStatusMessage(true);

            IConfiguration configuration = Substitute.For<IConfiguration>();
            var sut = new IbHost(configuration);
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
            var sut = new IbHost(configuration);
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

        [Fact]
        public void AddConnectionStatusMessageOnDisconnect()
        {
            ConnectionStatusMessage messageNotConnected = new ConnectionStatusMessage(false);
            ConnectionStatusMessage messageConnected = new ConnectionStatusMessage(true);

            IConfiguration configuration = Substitute.For<IConfiguration>();
            var sut = new IbHost(configuration);
            MainWindowViewModel viewModel = (MainWindowViewModel)FormatterServices
                .GetUninitializedObject(typeof(MainWindowViewModel));
            viewModel.Messages = new ObservableCollection<Message>();
            sut.ViewModel = viewModel;

            Reflection.CallMethod(
                sut,
                "_ibClient_NextValidId",
                new object[] { messageConnected });
            Reflection.CallMethod(
                sut,
                "_ibClient_ConnectionClosed",
                new object[] { });

            Assert.Equal(2, viewModel.Messages.Count);
            Assert.Equal(0, viewModel.Messages.Last().ReqId);
            var expectedBody = "DISCONNECTED!";
            Assert.Equal(expectedBody, viewModel.Messages.Last().Body);
            Assert.StartsWith(expectedBody, viewModel.ConnectionMessage);
            Assert.False(viewModel.Connected);
        }
    }
}
