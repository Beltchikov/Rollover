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
        ILogic? _logic;

        public ResponseProcessor(IDispatcherAbstraction dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Process(object message)
        {
            var messageType = message.GetType();

            // TODO Type is ReqIdAndResponses
            if(messageType == typeof(ErrorInfo))
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                _dispatcher.Invoke(() => _logic.InvokeError((message as ErrorInfo).ToString()));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            else
            { }

        }

        public void SetLogic(ILogic logic)
        {
            _logic = logic;
        }
    }
}
