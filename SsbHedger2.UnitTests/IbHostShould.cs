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
            //int reqId = 1;
            //int code = 2;
            //string message = "3";
            //Exception exception = new Exception("4");

            //var sut = new IbHost();
            //MainWindowViewModel viewModel = (MainWindowViewModel)FormatterServices
            //    .GetUninitializedObject(typeof(MainWindowViewModel));
            //viewModel.Messages = new ObservableCollection<Message>();
            //sut.ViewModel = viewModel;

            //Reflection.CallMethod(
            //    sut,
            //    "_ibClient_Error",
            //    new object[] { reqId, code, message, exception });

            //Assert.Single(viewModel.Messages);
            //Assert.Equal(reqId, viewModel.Messages.First().ReqId);
            //Assert.Equal($"Code:{code} message:{message} exception:{exception}",
            //    viewModel.Messages.First().Body);
        }
    }
}
