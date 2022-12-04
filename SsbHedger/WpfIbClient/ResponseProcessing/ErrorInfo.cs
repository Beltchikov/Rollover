using System;

namespace SsbHedger.ResponseProcessing
{
    public record ErrorInfo(
        int ReqId,
        int Code,
        string Message,
        Exception? Exception)
    {
        public override string ToString()
        {
            return $"ReqId:{ReqId} code:{Code} message:{Message} exception:{Exception}";
        }
    }

}
