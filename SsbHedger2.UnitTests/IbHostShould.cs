using SsbHedger2.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger2.UnitTests
{
    public class IbHostShould
    {
        //[Theory, AutoNSubstituteData]
        //public void AddErrorMessageToViewModel1(
        //    int reqId,
        //    int code,
        //    string message,
        //    Exception exception,
        //    IbHost sut)
        //{
        //    MainWindowViewModel viewModel = (MainWindowViewModel)FormatterServices
        //        .GetUninitializedObject(typeof(MainWindowViewModel));
        //    sut.ViewModel = viewModel;

        //    Reflection.CallMethod(
        //        sut,
        //        "_ibClient_Error",
        //        new object[] { reqId, code, message, exception });
        //    Assert.Single(viewModel.Messages);
        //}

        [Fact]
        public void AddErrorMessageToViewModel2()
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
    }
}
