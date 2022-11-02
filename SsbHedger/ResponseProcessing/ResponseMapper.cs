using System;
using System.Collections.Generic;

namespace SsbHedger.ResponseProcessing
{
    public class ResponseMapper : IResponseMapper
    {
        private List<ReqIdAndResponses> _responses = new List<ReqIdAndResponses>();

        public void AddResponse(object message)
        {
            if(message is ErrorInfo errorInfo)
            {
                if(errorInfo.ReqId < 0)
                {
                    _responses.Add(new ReqIdAndResponses(errorInfo.ReqId, new List<object> { errorInfo }));
                }
            }
            else
            {
                throw new ArgumentException($"Unknown message type {typeof(ErrorInfo)}");
            }
        }

        public List<ReqIdAndResponses> GetGrouppedResponses()
        {
            return _responses;
        }
    }
}
