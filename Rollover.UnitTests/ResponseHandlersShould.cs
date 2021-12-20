using IBSampleApp.messages;
using NSubstitute;
using Rollover.Ib;
using Rollover.Input;
using Rollover.Tracking;
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
            var portfolio = Substitute.For<IPortfolio>();

            var sut = ResponseHandlers.CreateInstance(inputQueue, portfolio);
            sut.OnError(id, errorCode, msg , ex);

            inputQueue.Received().Enqueue(Arg.Any<string>());
        }

        [Fact]
        public void CallEnqueueInNextValidId()
        {
            var inputQueue = Substitute.For<IInputQueue>();
            var portfolio = Substitute.For<IPortfolio>();

            var sut = ResponseHandlers.CreateInstance(inputQueue, portfolio);
            sut.NextValidId(new ConnectionStatusMessage(true));
            inputQueue.Received().Enqueue(Arg.Any<string>());
        }

        [Fact]
        public void CallEnqueueInManagedAccounts()
        {
            var inputQueue = Substitute.For<IInputQueue>();
            var portfolio = Substitute.For<IPortfolio>();

            var sut = ResponseHandlers.CreateInstance(inputQueue, portfolio);
            sut.ManagedAccounts(new ManagedAccountsMessage("accounts"));
            inputQueue.Received().Enqueue(Arg.Any<string>());
        }

        [Fact]
        public void CallEnqueueInOnPositionEndAfterOnPosition()
        {
            var inputQueue = Substitute.For<IInputQueue>();
            var portfolio = Substitute.For<IPortfolio>();

            var sut = ResponseHandlers.CreateInstance(inputQueue, portfolio);
            sut.OnPosition(new PositionMessage("account1", new IBApi.Contract(),2, 1000));
            sut.OnPosition(new PositionMessage("account2", new IBApi.Contract(),2, 1000));
            sut.OnPositionEnd();
            
            inputQueue.Received(3).Enqueue(Arg.Any<string>());
        }

        [Fact]
        public void CallEnqueueInOnPositionEnd()
        {
            var inputQueue = Substitute.For<IInputQueue>();
            var portfolio = Substitute.For<IPortfolio>();

            var sut = ResponseHandlers.CreateInstance(inputQueue, portfolio);
            sut.OnPositionEnd();
            inputQueue.Received().Enqueue(Arg.Any<string>());
        }

        [Fact]
        public void CallPortfolioAddInOnPositionEnd()
        {
            var inputQueue = Substitute.For<IInputQueue>();
            var portfolio = Substitute.For<IPortfolio>();
            var positionMessage = new PositionMessage("account", new IBApi.Contract(), 2, 1000);

            var sut = ResponseHandlers.CreateInstance(inputQueue, portfolio);
            sut.OnPosition(positionMessage);
            portfolio.Received().Add(positionMessage);
        }
    }
}
