using SsbHedger.ResponseProcessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger.UnitTests.ResponseProcessing
{
    public class ResponseProcessorShould
    {
        [Theory, AutoNSubstituteData]
        public void CallDispatcherInvoke(
            object message,
            ResponseProcessor sut)
        {
            sut.Process(message);
        }
    }
}
