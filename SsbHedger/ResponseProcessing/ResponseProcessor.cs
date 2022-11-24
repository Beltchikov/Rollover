using SsbHedger.Abstractions;
using SsbHedger.WpfIbClient;
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
        IWpfIbClient? _logic;

        public ResponseProcessor(IDispatcherAbstraction dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Process(ReqIdAndResponses reqIdAndResponses)
        {
            var reqId = reqIdAndResponses.ReqId;    
            foreach(var message in reqIdAndResponses.Responses)
            {
                var messageType = message.GetType();

                if (messageType == typeof(ErrorInfo))
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    _dispatcher.Invoke(() => _logic.InvokeError(reqId, (message as ErrorInfo).ToString()));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                }
                else
                { }
            }
        }

        public void SetLogic(IWpfIbClient logic)
        {
            _logic = logic;
        }
    }
}
