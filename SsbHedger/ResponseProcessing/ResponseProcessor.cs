using SsbHedger.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SsbHedger.ResponseProcessing
{
    public class ResponseProcessor : IResponseProcessor
    {
        IDispatcherAbstraction _dispatcher;

        public ResponseProcessor(IDispatcherAbstraction dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Process(object message)
        {
            _dispatcher.Invoke(() => { });
        }
    }
}
