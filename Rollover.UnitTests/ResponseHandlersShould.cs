using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rollover.UnitTests
{
    public class ResponseHandlersShould
    {
        [Fact]
        public void CallEnqueueInOnError()
        {
            int id = 20;
            int errorCode = 2104;
            string msg = "Test";
            Exception ex = new ArgumentException("SomeArgument");
            
            var inputQueue = Substitute.For<IInputQueue>();
            
            var sut = ResponseHandlers.CreateInstance(inputQueue);
            sut.OnError(id, errorCode, msg , ex);

            inputQueue.Received().Enqueue(Arg.Any<string>());
        }

        [Fact]
        public void CallEnqueueInNextValidId()
        {
            var inputQueue = Substitute.For<IInputQueue>();
            var sut = ResponseHandlers.CreateInstance(inputQueue);
            sut.NextValidId(new ConnectionStatusMessage(true));
            inputQueue.Received().Enqueue(Arg.Any<string>());
        }

        [Fact]
        public void CallEnqueueInManagedAccounts()
        {
            var inputQueue = Substitute.For<IInputQueue>();
            var sut = ResponseHandlers.CreateInstance(inputQueue);
            sut.ManagedAccounts(new ManagedAccountsMessage("accounts"));
            inputQueue.Received().Enqueue(Arg.Any<string>());
        }

        [Fact]
        public void CallEnqueueInOnPosition()
        {
            var inputQueue = Substitute.For<IInputQueue>();
            var sut = ResponseHandlers.CreateInstance(inputQueue);
            sut.OnPosition(new PositionMessage("account", new IBApi.Contract(),2, 1000));
            inputQueue.Received().Enqueue(Arg.Any<string>());
        }

        [Fact]
        public void CallEnqueueInOnPositionEnd()
        {
            var inputQueue = Substitute.For<IInputQueue>();
            var sut = ResponseHandlers.CreateInstance(inputQueue);
            sut.OnPositionEnd();
            inputQueue.Received().Enqueue(Arg.Any<string>());
        }
    }
}
