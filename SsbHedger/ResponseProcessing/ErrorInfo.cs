using System;

namespace SsbHedger.ResponseProcessing
{
    public record ErrorInfo(
        int ReqId,
        int Code,
        string Message,
        Exception? exception)
    {
        public override string ToString()
        {
            return $"ReqId:{ReqId} code:{Code} message:{Message} exception:{exception}";
        }
    }

}
