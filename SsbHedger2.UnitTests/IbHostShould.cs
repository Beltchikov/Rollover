using IbClient.messages;
using SsbHedger2.Model;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace SsbHedger2.UnitTests
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

            var sut = new IbHost();
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

            var sut = new IbHost();
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

            var sut = new IbHost();
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
        }
    }
}
