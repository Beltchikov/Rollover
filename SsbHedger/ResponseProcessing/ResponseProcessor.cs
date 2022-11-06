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
        ILogic _logic;

        public ResponseProcessor(IDispatcherAbstraction dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Process(object message)
        {
            //switch(message.GetType())
            //{
            //    case typeof(ErrorInfo):
            //        break;
            //}
            
            _dispatcher.Invoke(() => { });
        }

        public void SetLogic(ILogic logic)
        {
            _logic = logic;
        }
    }
}
