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
    }
}
