using System;

namespace SsbHedger.ResponseProcessing
{
    public class ResponseMapper : IResponseMapper
    {
        public void AddResponse(object message)
        {
            if(message is ErrorInfo)
            {
                //TODO
            }

            throw new ArgumentException($"Unknown message type {typeof(ErrorInfo)}");
        }
    }
}
