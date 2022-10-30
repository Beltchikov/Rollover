using System;

namespace SsbHedger.ResponseProcessing
{
    public record ErrorMessage(
        int ReqId,
        int Code,
        string Message,
        Exception exception);
    
}
