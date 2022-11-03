using IbClient.messages;
using System;
using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing.Mapper
{
    public class ResponseMapper : IResponseMapper
    {
        private List<ReqIdAndResponses> _responses = new List<ReqIdAndResponses>();

        public void AddResponse(object message)
        {
            if (message is ErrorInfo errorInfo)
            {
                if (errorInfo.ReqId < 0)
                {
                    _responses.Add(new ReqIdAndResponses(errorInfo.ReqId, new List<object> { errorInfo }));
                }
            }
            else if (message is OpenOrderMessage openOrderMessage)
            {
                _responses.Add(new ReqIdAndResponses(openOrderMessage.OrderId, new List<object> { openOrderMessage }));
            }
            else if (message is ConnectionStatusMessage connectionStatusMessage)
            {
                _responses.Add(new ReqIdAndResponses(-1, new List<object> { connectionStatusMessage }));
            }
            else if (message is ManagedAccountsMessage managedAccountsMessage)
            {
                _responses.Add(new ReqIdAndResponses(-1, new List<object> { managedAccountsMessage }));
            }
            else
            {
                throw new ArgumentException($"Unknown message type {message.GetType()}");
            }
        }

        public List<ReqIdAndResponses> GetGrouppedResponses()
        {
            var responsesCopy = new List<ReqIdAndResponses>(_responses);
            _responses.Clear();
            return responsesCopy;
        }

        public int Count => _responses.Count;
    }
}
